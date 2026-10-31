namespace CineTec.Core.DTOs
{
    public class UsuarioDto
    {
        public string Nombre { get; set; } = null!;
        public string? Email { get; set; }
        public string? Correo { get; set; }
        public string Password { get; set; } = null!;
        public string? Rol { get; set; }

        public string GetEmail() => !string.IsNullOrWhiteSpace(Email) ? Email : (Correo ?? string.Empty);
    }
}
