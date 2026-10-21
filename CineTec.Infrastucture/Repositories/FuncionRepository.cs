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
    public class FuncionRepository : IFuncionRepository
    {
        private readonly CineTecContext _cineTecContext;

        public FuncionRepository(CineTecContext cineTecContext)
        {
            _cineTecContext = cineTecContext;
        }

        public async Task<IEnumerable<FuncionDto>> GetActiveShowtimesAsync(string? fecha)
        {
            var query = _cineTecContext.Funciones
                .Include(f => f.Pelicula)
                .Include(f => f.Sala)
                .Where(f => f.Estado == "Activa" && f.FechaHoraInicio >= DateTime.Now);

            if (!string.IsNullOrEmpty(fecha) && DateTime.TryParse(fecha, out var fechaFiltro))
            {
                query = query.Where(f => f.FechaHoraInicio.Date == fechaFiltro.Date);
            }

            var funciones = await query.Select(f => new FuncionDto
            {
                FuncionId = f.Id,
                PeliculaTitulo = f.Pelicula.Titulo,
                Clasificacion = f.Pelicula.Clasificacion,
                SalaNombre = f.Sala.Nombre,
                FechaHoraInicio = f.FechaHoraInicio,
                FechaHoraFin = f.FechaHoraFin,
                PrecioBase = f.PrecioBase
            }).ToListAsync();

            return funciones;
        }

        public async Task<object?> GetSeatsByShowtimeIdAsync(int id)
        {
            var funcion = await _cineTecContext.Funciones
                .Include(f => f.Sala)
                .ThenInclude(s => s.Butacas)
                .FirstOrDefaultAsync(f => f.Id == id && f.Estado == "Activa");

            if (funcion == null)
            {
                return null;
            }

            var butacas = funcion.Sala.Butacas.Select(b => new ButacaEstadoDto
            {
                Id = b.Id,
                Codigo = b.Codigo,
                Fila = b.Fila,
                Columna = b.Columna,
                Estado = "libre"
            }).ToList();

            var resultado = new
            {
                FuncionId = funcion.Id,
                Sala = funcion.Sala.Nombre,
                Filas = funcion.Sala.Filas,
                Columnas = funcion.Sala.Columnas,
                Butacas = butacas
            };

            return resultado;
        }
    }
}