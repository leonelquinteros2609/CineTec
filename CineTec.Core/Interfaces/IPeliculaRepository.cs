using CineTec.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CineTec.Core.Interfaces
{
    public interface IPeliculaRepository
    {
        Task<IEnumerable<Pelicula>> GetAllActiveMoviesAsync();
        Task<Pelicula?> GetByIdAsync(int id);
        Task InsertAsync(Pelicula pelicula);
        Task UpdateAsync(Pelicula pelicula);
    }
}