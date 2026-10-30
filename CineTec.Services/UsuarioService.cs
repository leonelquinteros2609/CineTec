using CineTec.Core.DTOs;
using CineTec.Core.Entities;
using CineTec.Core.Interfaces;
using CineTec.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace CineTec.Services
{
    public class UsuarioService : AuthService
    {
        public UsuarioService(CineTecContext context, IConfiguration configuration) : base(context, configuration)
        {
        }
    }
}
