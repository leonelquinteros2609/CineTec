using Microsoft.EntityFrameworkCore;
using CineTec.Core.Entities;
using CineTec.Core.Interfaces;
using CineTec.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CineTec.Infrastructure.Repositories
{
    public class PeliculaRepository : BaseRepository<Pelicula>, IPeliculaRepository
    {
        public PeliculaRepository(CineTecContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Pelicula>> GetAllActiveMoviesAsync()
        {
            return await _entities
                .Where(p => p.Estado == "Activa")
                .ToListAsync();
        }
    }
}
