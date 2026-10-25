using CineTec.Core.Entities;
using CineTec.Core.Interfaces;
using CineTec.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CineTec.Infrastructure.Repositories;

public class ReservaRepository : IReservaRepository
{
    private readonly CineTecContext _context;

    public ReservaRepository(CineTecContext context)
    {
        _context = context;
    }

    public async Task<Reserva?> GetByIdAsync(int id)
    {
        return await _context.Reservas
            .Include(r => r.Funcion).ThenInclude(f => f.Pelicula)
            .Include(r => r.Funcion).ThenInclude(f => f.Sala)
            .Include(r => r.ReservaButacas).ThenInclude(rb => rb.Butaca)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Reserva>> GetByUsuarioIdAsync(int usuarioId)
    {
        return await _context.Reservas
            .Include(r => r.Funcion).ThenInclude(f => f.Pelicula)
            .Include(r => r.Funcion).ThenInclude(f => f.Sala)
            .Include(r => r.ReservaButacas).ThenInclude(rb => rb.Butaca)
            .Where(r => r.UsuarioId == usuarioId)
            .ToListAsync();
    }

    public async Task<IEnumerable<int>> GetButacasOcupadasByFuncionIdAsync(int funcionId)
    {
        return await _context.ReservaButacas
            .Where(rb => rb.Reserva.FuncionId == funcionId && rb.Reserva.Estado == "Confirmada")
            .Select(rb => rb.ButacaId)
            .ToListAsync();
    }

    public async Task InsertAsync(Reserva reserva)
    {
        await _context.Reservas.AddAsync(reserva);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Reserva reserva)
    {
        _context.Reservas.Update(reserva);
        await _context.SaveChangesAsync();
    }
}