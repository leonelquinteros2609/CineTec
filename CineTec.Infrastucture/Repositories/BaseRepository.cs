using Microsoft.EntityFrameworkCore;
using CineTec.Core.Entities;
using CineTec.Core.Interfaces;
using CineTec.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CineTec.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T>
        where T : BaseEntity
    {
        protected readonly CineTecContext _context;
        protected readonly DbSet<T> _entities;

        public BaseRepository(CineTecContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAll()
        {
            return await _entities.ToListAsync();
        }

        public virtual async Task<T?> GetById(int id)
        {
            return await _entities.FindAsync(id);
        }

        public virtual async Task Add(T entity)
        {
            _entities.Add(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task Update(T entity)
        {
            _entities.Update(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task Delete(int id)
        {
            var entity = await GetById(id);
            if (entity != null)
            {
                _entities.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public virtual IQueryable<T> Query()
        {
            return _entities.AsQueryable();
        }
    }
}
