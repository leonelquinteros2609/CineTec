using System;

namespace CineTec.Core.DTOs
{
    public class FuncionDto
    {
        public int FuncionId { get; set; }
        public string PeliculaTitulo { get; set; } = null!;
        public string? Clasificacion { get; set; }
        public string SalaNombre { get; set; } = null!;
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }
        public decimal PrecioBase { get; set; }
    }
}