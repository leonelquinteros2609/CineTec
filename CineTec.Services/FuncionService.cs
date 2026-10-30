using AutoMapper;
using Microsoft.EntityFrameworkCore;
using CineTec.Core.DTOs;
using CineTec.Core.Entities;
using CineTec.Core.Exceptions;
using CineTec.Core.Interfaces;
using CineTec.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CineTec.Services
{
    public class FuncionService : IFuncionService
    {
        private readonly CineTecContext _context;
        private readonly IMapper _mapper;

        public FuncionService(CineTecContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FuncionDto>> GetActiveShowtimesAsync(string? fecha, int? peliculaId = null)
        {
            var now = DateTime.Now;

            // RN-04: Solo se pueden consultar para reserva las funciones activas cuya hora de inicio sea futura
            var query = _context.Funciones
                .Include(f => f.Pelicula)
                .Include(f => f.Sala)
                .Where(f => f.Estado == "Activa" && f.FechaHoraInicio > now);

            if (peliculaId.HasValue && peliculaId.Value > 0)
            {
                query = query.Where(f => f.PeliculaId == peliculaId.Value);
            }

            if (!string.IsNullOrWhiteSpace(fecha) && DateTime.TryParse(fecha, out var parsedDate))
            {
                query = query.Where(f => f.FechaHoraInicio.Date == parsedDate.Date);
            }

            var funciones = await query.OrderBy(f => f.FechaHoraInicio).ToListAsync();
            return _mapper.Map<IEnumerable<FuncionDto>>(funciones);
        }

        public async Task<IEnumerable<FuncionDto>> GetAllShowtimesAsync()
        {
            var funciones = await _context.Funciones
                .Include(f => f.Pelicula)
                .Include(f => f.Sala)
                .OrderByDescending(f => f.FechaHoraInicio)
                .ToListAsync();

            return _mapper.Map<IEnumerable<FuncionDto>>(funciones);
        }

        public async Task<FuncionDto> GetShowtimeByIdAsync(int id)
        {
            var funcion = await _context.Funciones
                .Include(f => f.Pelicula)
                .Include(f => f.Sala)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (funcion == null)
            {
                throw new BusinessException("Función no encontrada", HttpStatusCode.NotFound);
            }

            return _mapper.Map<FuncionDto>(funcion);
        }

        public async Task<MapaButacasDto> GetSeatsMapByShowtimeIdAsync(int showtimeId)
        {
            var funcion = await _context.Funciones
                .Include(f => f.Pelicula)
                .Include(f => f.Sala)
                    .ThenInclude(s => s.Butacas)
                .FirstOrDefaultAsync(f => f.Id == showtimeId && f.Estado == "Activa");

            if (funcion == null)
            {
                throw new BusinessException("Función no encontrada o inactiva (RN-04)", HttpStatusCode.NotFound);
            }

            var now = DateTime.Now;

            // RN-01 y RN-02:
            // Butacas ocupadas: compras Pagadas
            // Butacas reservadas: compras Pendientes con fecha de expiración mayor a la actual (menos de 10 min)
            var entradasActivas = await _context.Entradas
                .Include(e => e.Compra)
                .Where(e => e.FuncionId == showtimeId &&
                            ((e.Compra.Estado == "Pagada" && e.Estado != "Cancelada") ||
                             (e.Compra.Estado == "Pendiente" && e.Compra.FechaExpiracion > now && e.Estado != "Cancelada")))
                .ToListAsync();

            var butacasEstado = funcion.Sala.Butacas
                .OrderBy(b => b.Fila)
                .ThenBy(b => b.Columna)
                .Select(b =>
                {
                    var entrada = entradasActivas.FirstOrDefault(e => e.ButacaId == b.Id);
                    string estado = "libre";

                    if (entrada != null)
                    {
                        if (entrada.Compra.Estado == "Pagada")
                        {
                            estado = "ocupada";
                        }
                        else if (entrada.Compra.Estado == "Pendiente")
                        {
                            estado = "reservada";
                        }
                    }

                    return new ButacaEstadoDto
                    {
                        Id = b.Id,
                        Codigo = b.Codigo,
                        Fila = b.Fila,
                        Columna = b.Columna,
                        Estado = estado
                    };
                }).ToList();

            return new MapaButacasDto
            {
                FuncionId = funcion.Id,
                PeliculaTitulo = funcion.Pelicula.Titulo,
                Sala = funcion.Sala.Nombre,
                Filas = funcion.Sala.Filas,
                Columnas = funcion.Sala.Columnas,
                Butacas = butacasEstado
            };
        }

        public async Task<FuncionDto> CreateShowtimeAsync(CrearFuncionDto dto)
        {
            var pelicula = await _context.Peliculas.FindAsync(dto.PeliculaId);
            if (pelicula == null || pelicula.Estado != "Activa")
            {
                throw new BusinessException("La película especificada no existe o no está activa.", HttpStatusCode.BadRequest);
            }

            var sala = await _context.Salas.FindAsync(dto.SalaId);
            if (sala == null)
            {
                throw new BusinessException("La sala especificada no existe.", HttpStatusCode.BadRequest);
            }

            if (dto.FechaHoraInicio <= DateTime.Now)
            {
                throw new BusinessException("La fecha y hora de inicio de la función debe ser futura.", HttpStatusCode.BadRequest);
            }

            if (dto.PrecioBase <= 0)
            {
                throw new BusinessException("El precio base de la función debe ser mayor que cero.", HttpStatusCode.BadRequest);
            }

            var fechaHoraFin = dto.FechaHoraInicio.AddMinutes(pelicula.DuracionMin);

            // RN-07: Dos funciones de una misma sala no pueden solaparse; entre una y otra debe haber al menos 15 minutos de limpieza.
            var tiempoLimpieza = TimeSpan.FromMinutes(15);
            var nuevoInicioConBuffer = dto.FechaHoraInicio;
            var nuevoFinConBuffer = fechaHoraFin.Add(tiempoLimpieza);

            var conflicto = await _context.Funciones
                .AnyAsync(f => f.SalaId == dto.SalaId &&
                               f.Estado == "Activa" &&
                               (dto.FechaHoraInicio < f.FechaHoraFin.Add(tiempoLimpieza) &&
                                f.FechaHoraInicio < nuevoFinConBuffer));

            if (conflicto)
            {
                throw new BusinessException(
                    "RN-07: Conflicto de horarios. Dos funciones en la misma sala no pueden solaparse y debe haber al menos 15 minutos de limpieza entre ellas.",
                    HttpStatusCode.Conflict);
            }

            var funcion = new Funcion
            {
                PeliculaId = dto.PeliculaId,
                SalaId = dto.SalaId,
                FechaHoraInicio = dto.FechaHoraInicio,
                FechaHoraFin = fechaHoraFin,
                PrecioBase = dto.PrecioBase,
                Estado = "Activa"
            };

            _context.Funciones.Add(funcion);
            await _context.SaveChangesAsync();

            // Cargar relaciones para el DTO
            await _context.Entry(funcion).Reference(f => f.Pelicula).LoadAsync();
            await _context.Entry(funcion).Reference(f => f.Sala).LoadAsync();

            return _mapper.Map<FuncionDto>(funcion);
        }

        public async Task<FuncionDto> UpdateShowtimeAsync(int id, ActualizarFuncionDto dto)
        {
            var funcion = await _context.Funciones.FindAsync(id);
            if (funcion == null)
            {
                throw new BusinessException("Función no encontrada.", HttpStatusCode.NotFound);
            }

            var pelicula = await _context.Peliculas.FindAsync(dto.PeliculaId);
            if (pelicula == null)
            {
                throw new BusinessException("La película especificada no existe.", HttpStatusCode.BadRequest);
            }

            var sala = await _context.Salas.FindAsync(dto.SalaId);
            if (sala == null)
            {
                throw new BusinessException("La sala especificada no existe.", HttpStatusCode.BadRequest);
            }

            var fechaHoraFin = dto.FechaHoraInicio.AddMinutes(pelicula.DuracionMin);

            // RN-07: Solapamiento con 15 minutos de limpieza
            var tiempoLimpieza = TimeSpan.FromMinutes(15);
            var nuevoFinConBuffer = fechaHoraFin.Add(tiempoLimpieza);

            var conflicto = await _context.Funciones
                .AnyAsync(f => f.Id != id &&
                               f.SalaId == dto.SalaId &&
                               f.Estado == "Activa" &&
                               (dto.FechaHoraInicio < f.FechaHoraFin.Add(tiempoLimpieza) &&
                                f.FechaHoraInicio < nuevoFinConBuffer));

            if (conflicto)
            {
                throw new BusinessException(
                    "RN-07: Conflicto de horarios. Dos funciones en la misma sala no pueden solaparse y debe haber al menos 15 minutos de limpieza entre ellas.",
                    HttpStatusCode.Conflict);
            }

            funcion.PeliculaId = dto.PeliculaId;
            funcion.SalaId = dto.SalaId;
            funcion.FechaHoraInicio = dto.FechaHoraInicio;
            funcion.FechaHoraFin = fechaHoraFin;
            funcion.PrecioBase = dto.PrecioBase;
            if (!string.IsNullOrWhiteSpace(dto.Estado))
            {
                funcion.Estado = dto.Estado;
            }

            await _context.SaveChangesAsync();

            await _context.Entry(funcion).Reference(f => f.Pelicula).LoadAsync();
            await _context.Entry(funcion).Reference(f => f.Sala).LoadAsync();

            return _mapper.Map<FuncionDto>(funcion);
        }

        public async Task DeleteShowtimeAsync(int id)
        {
            var funcion = await _context.Funciones
                .Include(f => f.Entradas)
                    .ThenInclude(e => e.Compra)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (funcion == null)
            {
                throw new BusinessException("Función no encontrada.", HttpStatusCode.NotFound);
            }

            // RN-08: No se puede eliminar una función que tenga entradas vendidas; solo se desactiva o cancela.
            var tieneEntradasVendidas = funcion.Entradas.Any(e => e.Compra.Estado == "Pagada" && e.Estado != "Cancelada");
            if (tieneEntradasVendidas)
            {
                throw new BusinessException(
                    "RN-08: No se puede eliminar una función que tenga entradas vendidas; solo se puede cancelar.",
                    HttpStatusCode.BadRequest);
            }

            if (funcion.Entradas.Any())
            {
                funcion.Estado = "Cancelada";
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.Funciones.Remove(funcion);
                await _context.SaveChangesAsync();
            }
        }
    }
}
