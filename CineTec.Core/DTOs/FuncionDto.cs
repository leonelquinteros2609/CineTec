using System;
using System.Collections.Generic;

namespace CineTec.Core.DTOs
{
    public class FuncionDto
    {
        public int FuncionId { get; set; }
        public int Id { get => FuncionId; set => FuncionId = value; }
        public int PeliculaId { get; set; }
        public string PeliculaTitulo { get; set; } = string.Empty;
        public string? Clasificacion { get; set; }
        public int SalaId { get; set; }
        public string SalaNombre { get; set; } = string.Empty;
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }
        public decimal PrecioBase { get; set; }
        public string Estado { get; set; } = "Activa";
    }

    public class CrearFuncionDto
    {
        public int PeliculaId { get; set; }
        public int SalaId { get; set; }
        public DateTime FechaHoraInicio { get; set; }
        public decimal PrecioBase { get; set; }
    }

    public class ActualizarFuncionDto
    {
        public int Id { get; set; }
        public int PeliculaId { get; set; }
        public int SalaId { get; set; }
        public DateTime FechaHoraInicio { get; set; }
        public decimal PrecioBase { get; set; }
        public string Estado { get; set; } = "Activa";
    }

    public class MapaButacasDto
    {
        public int FuncionId { get; set; }
        public string PeliculaTitulo { get; set; } = string.Empty;
        public string Sala { get; set; } = string.Empty;
        public int Filas { get; set; }
        public int Columnas { get; set; }
        public List<ButacaEstadoDto> Butacas { get; set; } = new();
    }
}
