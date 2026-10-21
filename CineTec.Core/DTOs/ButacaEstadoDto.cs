namespace CineTec.Core.DTOs
{
    public class ButacaEstadoDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Fila { get; set; } = null!;
        public int Columna { get; set; }
        public string Estado { get; set; } = "libre";
    }
}