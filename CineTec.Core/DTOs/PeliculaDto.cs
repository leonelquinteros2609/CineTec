namespace CineTec.Core.DTOs
{
    public class PeliculaDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Sinopsis { get; set; }
        public int DuracionMin { get; set; }
        public int DuracionMinutos { get => DuracionMin; set => DuracionMin = value; }
        public string Clasificacion { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public string Estado { get; set; } = "Activa";
    }

    public class CrearPeliculaDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string? Sinopsis { get; set; }
        public int DuracionMin { get; set; }
        public string Clasificacion { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
    }

    public class ActualizarPeliculaDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Sinopsis { get; set; }
        public int DuracionMin { get; set; }
        public string Clasificacion { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public string Estado { get; set; } = "Activa";
    }
}
