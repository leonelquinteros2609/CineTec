namespace CineTec.Core.DTOs
{
    public class UsuarioDto
    {
        public string Nombre { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Rol { get; set; }
    }
}