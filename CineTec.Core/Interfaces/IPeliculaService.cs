using CineTec.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CineTec.Core.Interfaces
{
    public interface IPeliculaService
    {
        Task<IEnumerable<PeliculaDto>> GetAllActiveMoviesAsync();
        Task<IEnumerable<PeliculaDto>> GetAllMoviesAsync();
        Task<PeliculaDto> GetMovieByIdAsync(int id);
        Task<PeliculaDto> CreateMovieAsync(CrearPeliculaDto dto);
        Task<PeliculaDto> UpdateMovieAsync(int id, ActualizarPeliculaDto dto);
        Task DeleteMovieAsync(int id);
    }
}
