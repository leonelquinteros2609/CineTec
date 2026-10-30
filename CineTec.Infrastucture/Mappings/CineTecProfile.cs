using AutoMapper;
using CineTec.Core.DTOs;
using CineTec.Core.Entities;

namespace CineTec.Infrastructure.Mappings
{
    public class CineTecProfile : Profile
    {
        public CineTecProfile()
        {
            // Usuario
            CreateMap<Usuario, UsuarioDto>()
                .ForMember(dest => dest.Password, opt => opt.Ignore());
            CreateMap<UsuarioDto, Usuario>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.GetEmail()))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            // Película
            CreateMap<Pelicula, PeliculaDto>().ReverseMap();
            CreateMap<CrearPeliculaDto, Pelicula>();
            CreateMap<ActualizarPeliculaDto, Pelicula>();

            // Sala
            CreateMap<Sala, SalaDto>().ReverseMap();

            // Función
            CreateMap<Funcion, FuncionDto>()
                .ForMember(dest => dest.FuncionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PeliculaTitulo, opt => opt.MapFrom(src => src.Pelicula != null ? src.Pelicula.Titulo : string.Empty))
                .ForMember(dest => dest.Clasificacion, opt => opt.MapFrom(src => src.Pelicula != null ? src.Pelicula.Clasificacion : string.Empty))
                .ForMember(dest => dest.SalaNombre, opt => opt.MapFrom(src => src.Sala != null ? src.Sala.Nombre : string.Empty));
            CreateMap<CrearFuncionDto, Funcion>();
            CreateMap<ActualizarFuncionDto, Funcion>();

            // Butaca
            CreateMap<Butaca, ButacaEstadoDto>()
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(_ => "libre"));

            // Entrada
            CreateMap<Entrada, EntradaDetalleDto>()
                .ForMember(dest => dest.EntradaId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CodigoButaca, opt => opt.MapFrom(src => src.Butaca != null ? src.Butaca.Codigo : string.Empty))
                .ForMember(dest => dest.Fila, opt => opt.MapFrom(src => src.Butaca != null ? src.Butaca.Fila : string.Empty))
                .ForMember(dest => dest.Columna, opt => opt.MapFrom(src => src.Butaca != null ? src.Butaca.Columna : 0));

            // Pago
            CreateMap<Pago, PagoDetalleDto>()
                .ForMember(dest => dest.PagoId, opt => opt.MapFrom(src => src.Id));

            // Compra
            CreateMap<Compra, CompraDetalleDto>()
                .ForMember(dest => dest.CompraId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UsuarioNombre, opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.Nombre : string.Empty))
                .ForMember(dest => dest.UsuarioEmail, opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.Email : string.Empty))
                .ForMember(dest => dest.PeliculaTitulo, opt => opt.MapFrom(src => src.Funcion != null && src.Funcion.Pelicula != null ? src.Funcion.Pelicula.Titulo : string.Empty))
                .ForMember(dest => dest.SalaNombre, opt => opt.MapFrom(src => src.Funcion != null && src.Funcion.Sala != null ? src.Funcion.Sala.Nombre : string.Empty))
                .ForMember(dest => dest.FechaHoraInicio, opt => opt.MapFrom(src => src.Funcion != null ? src.Funcion.FechaHoraInicio : default))
                .ForMember(dest => dest.FechaHoraFin, opt => opt.MapFrom(src => src.Funcion != null ? src.Funcion.FechaHoraFin : default));

            CreateMap<Compra, ReservaResponseDto>()
                .ForMember(dest => dest.CompraId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PeliculaTitulo, opt => opt.MapFrom(src => src.Funcion != null && src.Funcion.Pelicula != null ? src.Funcion.Pelicula.Titulo : string.Empty))
                .ForMember(dest => dest.SalaNombre, opt => opt.MapFrom(src => src.Funcion != null && src.Funcion.Sala != null ? src.Funcion.Sala.Nombre : string.Empty))
                .ForMember(dest => dest.FechaHoraInicio, opt => opt.MapFrom(src => src.Funcion != null ? src.Funcion.FechaHoraInicio : default))
                .ForMember(dest => dest.MinutosRestantes, opt => opt.MapFrom(src => Math.Max(0, (src.FechaExpiracion - System.DateTime.Now).TotalMinutes)));
        }
    }
}
