using Microsoft.EntityFrameworkCore;
using CineTec.Core.Entities;
using CineTec.Core.Interfaces;
using CineTec.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CineTec.Infrastructure.Repositories
{
    public class PeliculaRepository : IPeliculaRepository
    {
        private readonly CineTecContext _cineTecContext;

        public PeliculaRepository(CineTecContext cineTecContext)
        {
            _cineTecContext = cineTecContext;
        }

        public async Task<IEnumerable<Pelicula>> GetAllActiveMoviesAsync()
        {
            var peliculas = await _cineTecContext.Peliculas
                .Where(p => p.Estado == "Activa")
                .ToListAsync();

            return peliculas;
        }

        public Task<Pelicula?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(Pelicula pelicula)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Pelicula pelicula)
        {
            throw new NotImplementedException();
        }
    }
}