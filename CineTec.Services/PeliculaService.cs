using AutoMapper;
using Microsoft.EntityFrameworkCore;
using CineTec.Core.DTOs;
using CineTec.Core.Entities;
using CineTec.Core.Exceptions;
using CineTec.Core.Interfaces;
using CineTec.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CineTec.Services
{
    public class PeliculaService : IPeliculaService
    {
        private readonly CineTecContext _context;
        private readonly IMapper _mapper;

        public PeliculaService(CineTecContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PeliculaDto>> GetAllActiveMoviesAsync()
        {
            var peliculas = await _context.Peliculas
                .Where(p => p.Estado == "Activa")
                .ToListAsync();

            return _mapper.Map<IEnumerable<PeliculaDto>>(peliculas);
        }

        public async Task<IEnumerable<PeliculaDto>> GetAllMoviesAsync()
        {
            var peliculas = await _context.Peliculas.ToListAsync();
            return _mapper.Map<IEnumerable<PeliculaDto>>(peliculas);
        }

        public async Task<PeliculaDto> GetMovieByIdAsync(int id)
        {
            var pelicula = await _context.Peliculas.FindAsync(id);
            if (pelicula == null)
            {
                throw new BusinessException("Película no encontrada", HttpStatusCode.NotFound);
            }

            return _mapper.Map<PeliculaDto>(pelicula);
        }

        public async Task<PeliculaDto> CreateMovieAsync(CrearPeliculaDto dto)
        {
            var pelicula = _mapper.Map<Pelicula>(dto);
            pelicula.Estado = "Activa";

            _context.Peliculas.Add(pelicula);
            await _context.SaveChangesAsync();

            return _mapper.Map<PeliculaDto>(pelicula);
        }

        public async Task<PeliculaDto> UpdateMovieAsync(int id, ActualizarPeliculaDto dto)
        {
            var pelicula = await _context.Peliculas.FindAsync(id);
            if (pelicula == null)
            {
                throw new BusinessException("Película no encontrada", HttpStatusCode.NotFound);
            }

            pelicula.Titulo = dto.Titulo;
            pelicula.Sinopsis = dto.Sinopsis;
            pelicula.DuracionMin = dto.DuracionMin;
            pelicula.Clasificacion = dto.Clasificacion;
            pelicula.Genero = dto.Genero;
            if (!string.IsNullOrWhiteSpace(dto.Estado))
            {
                pelicula.Estado = dto.Estado;
            }

            await _context.SaveChangesAsync();
            return _mapper.Map<PeliculaDto>(pelicula);
        }

        public async Task DeleteMovieAsync(int id)
        {
            var pelicula = await _context.Peliculas
                .Include(p => p.Funciones)
                    .ThenInclude(f => f.Entradas)
                        .ThenInclude(e => e.Compra)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pelicula == null)
            {
                throw new BusinessException("Película no encontrada", HttpStatusCode.NotFound);
            }

            // RN-08: No se puede eliminar una película o función que tenga entradas vendidas; solo se desactiva o cancela.
            var tieneEntradasVendidas = pelicula.Funciones.Any(f =>
                f.Entradas.Any(e => e.Compra.Estado == "Pagada" && e.Estado != "Cancelada"));

            if (tieneEntradasVendidas)
            {
                throw new BusinessException(
                    "RN-08: No se puede eliminar una película que tenga entradas vendidas; solo se puede desactivar cambiando su estado a 'Inactiva'.",
                    HttpStatusCode.BadRequest);
            }

            // Si tiene funciones asociadas sin ventas, desactivamos para mantener integridad referencial
            if (pelicula.Funciones.Any())
            {
                pelicula.Estado = "Inactiva";
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.Peliculas.Remove(pelicula);
                await _context.SaveChangesAsync();
            }
        }
    }
}
