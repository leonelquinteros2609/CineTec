using CineTec.Core.DTOs;
using CineTec.Core.Entities;

namespace CineTec.Core.Interfaces;

public interface IPeliculaService
{
    Task<Pelicula> CrearPeliculaAsync(PeliculaCreateUpdateDto dto);
    Task ActualizarPeliculaAsync(int id, PeliculaCreateUpdateDto dto);
    Task DarDeBajaPeliculaAsync(int id);
}