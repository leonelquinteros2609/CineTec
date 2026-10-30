using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using CineTec.Core.DTOs;
using CineTec.Core.Entities;
using CineTec.Core.Exceptions;
using CineTec.Core.Interfaces;
using CineTec.Infrastructure.Data;

namespace CineTec.Services
{
    public class BookingService : IBookingService
    {
        private readonly CineTecContext _context;
        private readonly IMapper _mapper;

        public BookingService(CineTecContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // CU-03: Reservar Butacas
        public async Task<ReservaResponseDto> CreateBookingAsync(int usuarioId, CrearReservaDto dto)
        {
            if (usuarioId <= 0)
            {
                throw new BusinessException("Usuario no válido.", HttpStatusCode.Unauthorized);
            }

            // RN-03: Una compra puede incluir entre 1 y 8 entradas.
            if (dto.ButacaIds == null || dto.ButacaIds.Count < 1 || dto.ButacaIds.Count > 8)
            {
                throw new BusinessException("RN-03: Una compra puede incluir entre 1 y 8 entradas.", HttpStatusCode.BadRequest);
            }

            // No permitir IDs de butacas duplicados en la misma solicitud
            if (dto.ButacaIds.Distinct().Count() != dto.ButacaIds.Count)
            {
                throw new BusinessException("No se pueden seleccionar butacas duplicadas en la misma reserva.", HttpStatusCode.BadRequest);
            }

            // RN-04: Solo funciones activas cuya hora de inicio sea futura
            var funcion = await _context.Funciones
                .Include(f => f.Pelicula)
                .Include(f => f.Sala)
                .FirstOrDefaultAsync(f => f.Id == dto.FuncionId && f.Estado == "Activa");

            if (funcion == null)
            {
                throw new BusinessException("La función seleccionada no existe o no está activa.", HttpStatusCode.NotFound);
            }

            if (funcion.FechaHoraInicio <= DateTime.Now)
            {
                throw new BusinessException("RN-04: Solo se pueden reservar funciones activas cuya hora de inicio sea futura.", HttpStatusCode.BadRequest);
            }

            // Validar que todas las butacas pertenezcan a la sala de la función
            var butacasSolicitadas = await _context.Butacas
                .Where(b => b.SalaId == funcion.SalaId && dto.ButacaIds.Contains(b.Id))
                .ToListAsync();

            if (butacasSolicitadas.Count != dto.ButacaIds.Count)
            {
                throw new BusinessException("Una o más butacas seleccionadas no existen o no corresponden a la sala de la función.", HttpStatusCode.BadRequest);
            }

            var now = DateTime.Now;

            // RN-01 y RN-02: Una butaca no puede estar reservada o vendida más de una vez para la misma función.
            // Una reserva pendiente expira a los 10 minutos si no se paga; al expirar, sus butacas quedan libres.
            var butacasOcupadas = await _context.Entradas
                .Include(e => e.Compra)
                .Include(e => e.Butaca)
                .Where(e => e.FuncionId == dto.FuncionId &&
                            dto.ButacaIds.Contains(e.ButacaId) &&
                            e.Estado != "Cancelada" &&
                            (e.Compra.Estado == "Pagada" ||
                             (e.Compra.Estado == "Pendiente" && e.Compra.FechaExpiracion > now)))
                .Select(e => e.Butaca.Codigo)
                .ToListAsync();

            if (butacasOcupadas.Any())
            {
                throw new BusinessException(
                    $"RN-01: Las siguientes butacas ya están reservadas o vendidas: {string.Join(", ", butacasOcupadas)}.",
                    HttpStatusCode.Conflict);
            }

            // RN-11: El total de la compra es la suma del precio de la función por cada entrada; el precio se congela al momento de la reserva
            var precioUnitario = funcion.PrecioBase;
            var total = precioUnitario * dto.ButacaIds.Count;
            var fechaCompra = DateTime.Now;
            var fechaExpiracion = fechaCompra.AddMinutes(10); // RN-02: 10 minutos de expiración

            var compra = new Compra
            {
                UsuarioId = usuarioId,
                FuncionId = funcion.Id,
                FechaCompra = fechaCompra,
                FechaExpiracion = fechaExpiracion,
                Total = total,
                Estado = "Pendiente"
            };

            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();

            foreach (var butaca in butacasSolicitadas)
            {
                var entrada = new Entrada
                {
                    CompraId = compra.Id,
                    FuncionId = funcion.Id,
                    ButacaId = butaca.Id,
                    Precio = precioUnitario,
                    CodigoUnico = null, // Se genera al momento de confirmar el pago (CU-04 / RN-09)
                    Estado = "Reservada"
                };
                _context.Entradas.Add(entrada);
            }

            await _context.SaveChangesAsync();

            // Cargar datos para respuesta
            var entradasDetalle = butacasSolicitadas.Select(b => new EntradaDetalleDto
            {
                ButacaId = b.Id,
                CodigoButaca = b.Codigo,
                Fila = b.Fila,
                Columna = b.Columna,
                Precio = precioUnitario,
                CodigoUnico = null,
                Estado = "Reservada"
            }).ToList();

            return new ReservaResponseDto
            {
                CompraId = compra.Id,
                FuncionId = funcion.Id,
                PeliculaTitulo = funcion.Pelicula.Titulo,
                SalaNombre = funcion.Sala.Nombre,
                FechaHoraInicio = funcion.FechaHoraInicio,
                Total = compra.Total,
                Estado = compra.Estado,
                FechaCompra = compra.FechaCompra,
                FechaExpiracion = compra.FechaExpiracion,
                MinutosRestantes = Math.Max(0, (compra.FechaExpiracion - DateTime.Now).TotalMinutes),
                Entradas = entradasDetalle
            };
        }

        // CU-04: Confirmar Compra de Entradas (Pagar)
        public async Task<CompraDetalleDto> PayBookingAsync(int usuarioId, int compraId, PagarReservaDto dto)
        {
            var compra = await _context.Compras
                .Include(c => c.Usuario)
                .Include(c => c.Funcion)
                    .ThenInclude(f => f.Pelicula)
                .Include(c => c.Funcion)
                    .ThenInclude(f => f.Sala)
                .Include(c => c.Entradas)
                    .ThenInclude(e => e.Butaca)
                .Include(c => c.Pagos)
                .FirstOrDefaultAsync(c => c.Id == compraId);

            if (compra == null)
            {
                throw new BusinessException("Compra / Reserva no encontrada.", HttpStatusCode.NotFound);
            }

            // RN-10: Un cliente solo puede consultar, pagar y cancelar sus propias compras
            if (compra.UsuarioId != usuarioId)
            {
                throw new BusinessException("RN-10: No tiene permiso para operar sobre esta compra.", HttpStatusCode.Forbidden);
            }

            if (compra.Estado == "Pagada")
            {
                throw new BusinessException("La compra ya se encuentra pagada.", HttpStatusCode.BadRequest);
            }

            if (compra.Estado == "Cancelada")
            {
                throw new BusinessException("La reserva fue cancelada previamente.", HttpStatusCode.BadRequest);
            }

            var now = DateTime.Now;

            // RN-02: Una reserva pendiente expira a los 10 minutos si no se paga; al expirar, sus butacas quedan libres
            if (compra.Estado == "Expirada" || now > compra.FechaExpiracion)
            {
                compra.Estado = "Expirada";
                foreach (var entrada in compra.Entradas)
                {
                    entrada.Estado = "Cancelada";
                }
                await _context.SaveChangesAsync();

                throw new BusinessException("RN-02: La reserva ha expirado (límite de 10 minutos). Las butacas han sido liberadas.", HttpStatusCode.BadRequest);
            }

            // Registrar el pago simulado
            var metodoPago = string.IsNullOrWhiteSpace(dto.MetodoPago) ? "Tarjeta" : dto.MetodoPago;
            var pago = new Pago
            {
                CompraId = compra.Id,
                Monto = compra.Total,
                MetodoPago = metodoPago,
                FechaPago = now,
                Estado = "Completado"
            };
            _context.Pagos.Add(pago);

            // Actualizar estado de la compra a Pagada
            compra.Estado = "Pagada";

            // RN-09: Cada entrada tiene un código único generado
            foreach (var entrada in compra.Entradas)
            {
                entrada.Estado = "Pagada";
                entrada.CodigoUnico = GenerateUniqueTicketCode(entrada.Id, compra.Id);
            }

            await _context.SaveChangesAsync();

            return BuildCompraDetalleDto(compra);
        }

        // CU-05: Consultar Mis Compras
        public async Task<IEnumerable<CompraDetalleDto>> GetUserBookingsAsync(int usuarioId)
        {
            // RN-10: Un cliente solo puede consultar sus propias compras
            var compras = await _context.Compras
                .Include(c => c.Usuario)
                .Include(c => c.Funcion)
                    .ThenInclude(f => f.Pelicula)
                .Include(c => c.Funcion)
                    .ThenInclude(f => f.Sala)
                .Include(c => c.Entradas)
                    .ThenInclude(e => e.Butaca)
                .Include(c => c.Pagos)
                .Where(c => c.UsuarioId == usuarioId)
                .OrderByDescending(c => c.FechaCompra)
                .ToListAsync();

            // Si hay reservas pendientes que expiraron, marcarlas
            var now = DateTime.Now;
            bool cambio = false;
            foreach (var c in compras.Where(c => c.Estado == "Pendiente" && c.FechaExpiracion < now))
            {
                c.Estado = "Expirada";
                foreach (var e in c.Entradas)
                {
                    e.Estado = "Cancelada";
                }
                cambio = true;
            }

            if (cambio)
            {
                await _context.SaveChangesAsync();
            }

            return compras.Select(BuildCompraDetalleDto).ToList();
        }

        public async Task<CompraDetalleDto> GetBookingByIdAsync(int usuarioId, int compraId)
        {
            var compra = await _context.Compras
                .Include(c => c.Usuario)
                .Include(c => c.Funcion)
                    .ThenInclude(f => f.Pelicula)
                .Include(c => c.Funcion)
                    .ThenInclude(f => f.Sala)
                .Include(c => c.Entradas)
                    .ThenInclude(e => e.Butaca)
                .Include(c => c.Pagos)
                .FirstOrDefaultAsync(c => c.Id == compraId);

            if (compra == null)
            {
                throw new BusinessException("Compra no encontrada.", HttpStatusCode.NotFound);
            }

            // RN-10: Un cliente solo puede consultar sus propias compras
            if (compra.UsuarioId != usuarioId)
            {
                throw new BusinessException("RN-10: No tiene permiso para ver esta compra.", HttpStatusCode.Forbidden);
            }

            return BuildCompraDetalleDto(compra);
        }

        // CU-06: Cancelar Compra
        public async Task<CompraDetalleDto> CancelBookingAsync(int usuarioId, int compraId)
        {
            var compra = await _context.Compras
                .Include(c => c.Usuario)
                .Include(c => c.Funcion)
                    .ThenInclude(f => f.Pelicula)
                .Include(c => c.Funcion)
                    .ThenInclude(f => f.Sala)
                .Include(c => c.Entradas)
                    .ThenInclude(e => e.Butaca)
                .Include(c => c.Pagos)
                .FirstOrDefaultAsync(c => c.Id == compraId);

            if (compra == null)
            {
                throw new BusinessException("Compra no encontrada.", HttpStatusCode.NotFound);
            }

            // RN-10: Un cliente solo puede cancelar sus propias compras
            if (compra.UsuarioId != usuarioId)
            {
                throw new BusinessException("RN-10: No tiene permiso para cancelar esta compra.", HttpStatusCode.Forbidden);
            }

            if (compra.Estado == "Cancelada")
            {
                throw new BusinessException("La compra ya se encuentra cancelada.", HttpStatusCode.BadRequest);
            }

            // RN-09: Una compra con entradas usadas no puede cancelarse
            var tieneEntradasUsadas = compra.Entradas.Any(e => e.Estado == "Usada");
            if (tieneEntradasUsadas)
            {
                throw new BusinessException("RN-09: Una compra con entradas que ya fueron utilizadas en puerta no puede cancelarse.", HttpStatusCode.BadRequest);
            }

            // RN-05: Una compra solo puede cancelarse hasta 2 horas antes del inicio de la función
            var tiempoHastaFuncion = compra.Funcion.FechaHoraInicio - DateTime.Now;
            if (tiempoHastaFuncion < TimeSpan.FromHours(2))
            {
                throw new BusinessException(
                    "RN-05: Una compra solo puede cancelarse hasta 2 horas antes del inicio de la función.",
                    HttpStatusCode.BadRequest);
            }

            compra.Estado = "Cancelada";
            foreach (var entrada in compra.Entradas)
            {
                entrada.Estado = "Cancelada";
            }

            await _context.SaveChangesAsync();

            return BuildCompraDetalleDto(compra);
        }

        private static string GenerateUniqueTicketCode(int entradaId, int compraId)
        {
            var randomBytes = new byte[16];
            RandomNumberGenerator.Fill(randomBytes);
            var hash = Convert.ToHexString(randomBytes).ToUpperInvariant();
            return $"TKT-{compraId:D4}-{entradaId:D4}-{hash[..16]}";
        }

        private static CompraDetalleDto BuildCompraDetalleDto(Compra compra)
        {
            return new CompraDetalleDto
            {
                CompraId = compra.Id,
                UsuarioId = compra.UsuarioId,
                UsuarioNombre = compra.Usuario != null ? compra.Usuario.Nombre : string.Empty,
                UsuarioEmail = compra.Usuario != null ? compra.Usuario.Email : string.Empty,
                FuncionId = compra.FuncionId,
                PeliculaTitulo = compra.Funcion?.Pelicula?.Titulo ?? string.Empty,
                SalaNombre = compra.Funcion?.Sala?.Nombre ?? string.Empty,
                FechaHoraInicio = compra.Funcion?.FechaHoraInicio ?? default,
                FechaHoraFin = compra.Funcion?.FechaHoraFin ?? default,
                Total = compra.Total,
                Estado = compra.Estado,
                FechaCompra = compra.FechaCompra,
                FechaExpiracion = compra.FechaExpiracion,
                Entradas = compra.Entradas.Select(e => new EntradaDetalleDto
                {
                    EntradaId = e.Id,
                    ButacaId = e.ButacaId,
                    CodigoButaca = e.Butaca?.Codigo ?? string.Empty,
                    Fila = e.Butaca?.Fila ?? string.Empty,
                    Columna = e.Butaca?.Columna ?? 0,
                    Precio = e.Precio,
                    CodigoUnico = e.CodigoUnico,
                    Estado = e.Estado
                }).ToList(),
                Pagos = compra.Pagos.Select(p => new PagoDetalleDto
                {
                    PagoId = p.Id,
                    Monto = p.Monto,
                    MetodoPago = p.MetodoPago,
                    FechaPago = p.FechaPago,
                    Estado = p.Estado
                }).ToList()
            };
        }
    }
}
