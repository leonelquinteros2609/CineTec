using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CineTec.Core.DTOs;
using CineTec.Core.Exceptions;
using CineTec.Core.Interfaces;
using CineTec.Infrastructure.Data;

namespace CineTec.Services
{
    public class TicketService : ITicketService
    {
        private readonly CineTecContext _context;

        public TicketService(CineTecContext context)
        {
            _context = context;
        }

        public async Task<TicketValidadoDto> ValidateTicketAsync(ValidarTicketDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CodigoUnico))
            {
                throw new BusinessException("El código de entrada es obligatorio.", HttpStatusCode.BadRequest);
            }

            var codigo = dto.CodigoUnico.Trim();

            var entrada = await _context.Entradas
                .Include(e => e.Compra)
                .Include(e => e.Funcion)
                    .ThenInclude(f => f.Pelicula)
                .Include(e => e.Funcion)
                    .ThenInclude(f => f.Sala)
                .Include(e => e.Butaca)
                .FirstOrDefaultAsync(e => e.CodigoUnico == codigo);

            if (entrada == null)
            {
                throw new BusinessException("Código de entrada no encontrado en el sistema.", HttpStatusCode.NotFound);
            }

            // Validar que la compra asociada esté Pagada
            if (entrada.Compra.Estado != "Pagada")
            {
                throw new BusinessException($"La compra de esta entrada se encuentra en estado '{entrada.Compra.Estado}', no está pagada.", HttpStatusCode.BadRequest);
            }

            // RN-09: Solo puede validarse una vez
            if (entrada.Estado == "Usada")
            {
                throw new BusinessException("RN-09: Esta entrada ya fue utilizada anteriormente en puerta.", HttpStatusCode.BadRequest);
            }

            if (entrada.Estado == "Cancelada")
            {
                throw new BusinessException("Esta entrada pertenece a una compra que fue cancelada.", HttpStatusCode.BadRequest);
            }

            // RN-09: Únicamente el día de su función
            var today = DateTime.Today;
            if (entrada.Funcion.FechaHoraInicio.Date != today)
            {
                throw new BusinessException(
                    $"RN-09: La entrada solo puede validarse el día de la función ({entrada.Funcion.FechaHoraInicio:dd/MM/yyyy}). Fecha actual: {today:dd/MM/yyyy}.",
                    HttpStatusCode.BadRequest);
            }

            // Marcar como Usada
            entrada.Estado = "Usada";
            await _context.SaveChangesAsync();

            return new TicketValidadoDto
            {
                Valida = true,
                Mensaje = "Entrada válida. Acceso concedido a la sala.",
                EntradaId = entrada.Id,
                CodigoUnico = entrada.CodigoUnico!,
                Pelicula = entrada.Funcion.Pelicula.Titulo,
                Sala = entrada.Funcion.Sala.Nombre,
                Butaca = entrada.Butaca.Codigo,
                FechaHoraFuncion = entrada.Funcion.FechaHoraInicio,
                Estado = entrada.Estado
            };
        }
    }
}
