using Microsoft.EntityFrameworkCore;
using CineTec.Core.DTOs;
using CineTec.Core.Entities;
using CineTec.Core.Interfaces;
using CineTec.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CineTec.Infrastructure.Repositories
{
    public class FuncionRepository : BaseRepository<Funcion>, IFuncionRepository
    {
        public FuncionRepository(CineTecContext context) : base(context)
        {
        }

        public async Task<IEnumerable<FuncionDto>> GetActiveShowtimesAsync(string? fecha)
        {
            var now = DateTime.Now;
            var query = _entities
                .Include(f => f.Pelicula)
                .Include(f => f.Sala)
                .Where(f => f.Estado == "Activa" && f.FechaHoraInicio > now);

            if (!string.IsNullOrEmpty(fecha) && DateTime.TryParse(fecha, out var fechaFiltro))
            {
                query = query.Where(f => f.FechaHoraInicio.Date == fechaFiltro.Date);
            }

            var funciones = await query
                .OrderBy(f => f.FechaHoraInicio)
                .Select(f => new FuncionDto
                {
                    FuncionId = f.Id,
                    PeliculaId = f.PeliculaId,
                    PeliculaTitulo = f.Pelicula.Titulo,
                    Clasificacion = f.Pelicula.Clasificacion,
                    SalaId = f.SalaId,
                    SalaNombre = f.Sala.Nombre,
                    FechaHoraInicio = f.FechaHoraInicio,
                    FechaHoraFin = f.FechaHoraFin,
                    PrecioBase = f.PrecioBase,
                    Estado = f.Estado
                }).ToListAsync();

            return funciones;
        }

        public async Task<object?> GetSeatsByShowtimeIdAsync(int id)
        {
            var funcion = await _entities
                .Include(f => f.Pelicula)
                .Include(f => f.Sala)
                    .ThenInclude(s => s.Butacas)
                .FirstOrDefaultAsync(f => f.Id == id && f.Estado == "Activa");

            if (funcion == null)
            {
                return null;
            }

            var now = DateTime.Now;

            // Obtener las entradas de esta función que están vendidas (compra Pagada) o reservadas activas (compra Pendiente no expirada)
            var entradasActivas = await _context.Entradas
                .Include(e => e.Compra)
                .Where(e => e.FuncionId == id &&
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

            var resultado = new MapaButacasDto
            {
                FuncionId = funcion.Id,
                PeliculaTitulo = funcion.Pelicula.Titulo,
                Sala = funcion.Sala.Nombre,
                Filas = funcion.Sala.Filas,
                Columnas = funcion.Sala.Columnas,
                Butacas = butacasEstado
            };

            return resultado;
        }
    }
}
