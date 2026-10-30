namespace CineTec.Core.DTOs
{
    public class SalaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Filas { get; set; }
        public int Columnas { get; set; }
        public int Capacidad { get; set; }
    }
}
