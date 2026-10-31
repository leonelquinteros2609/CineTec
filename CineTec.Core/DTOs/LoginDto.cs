namespace CineTec.Core.DTOs
{
    public class LoginDto
    {
        public string? Email { get; set; }
        public string? Correo { get; set; }
        public string Password { get; set; } = null!;

        public string GetEmail() => !string.IsNullOrWhiteSpace(Email) ? Email : (Correo ?? string.Empty);
    }
}
