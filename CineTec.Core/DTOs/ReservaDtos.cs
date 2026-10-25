namespace CineTec.Core.DTOs;

public class CrearReservaDto
{
    public int UsuarioId { get; set; }
    public int FuncionId { get; set; }
    public List<int> ButacaIds { get; set; } = new();
}

public class ReservaDto
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public int FuncionId { get; set; }
    public string PeliculaTitulo { get; set; } = string.Empty;
    public string SalaNombre { get; set; } = string.Empty;
    public DateTime FechaHoraInicio { get; set; }
    public decimal MontoTotal { get; set; }
    public string Estado { get; set; } = string.Empty;
    public List<string> CodigosButacas { get; set; } = new();
}

public class PeliculaCreateUpdateDto
{
    public string Titulo { get; set; } = string.Empty;
    public string? Sinopsis { get; set; }
    public int DuracionMinutos { get; set; }
    public string Clasificacion { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
}