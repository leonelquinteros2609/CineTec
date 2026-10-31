using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using CineTec.Core.DTOs;
using CineTec.Core.Exceptions;
using CineTec.Core.Interfaces;
using CineTec.Infrastructure.Data;

namespace CineTec.Services
{
    public class SalaService : ISalaService
    {
        private readonly CineTecContext _context;
        private readonly IMapper _mapper;

        public SalaService(CineTecContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SalaDto>> GetAllRoomsAsync()
        {
            var salas = await _context.Salas.ToListAsync();
            return _mapper.Map<IEnumerable<SalaDto>>(salas);
        }

        public async Task<SalaDto> GetRoomByIdAsync(int id)
        {
            var sala = await _context.Salas.FindAsync(id);
            if (sala == null)
            {
                throw new BusinessException("Sala no encontrada.", HttpStatusCode.NotFound);
            }

            return _mapper.Map<SalaDto>(sala);
        }
    }
}
