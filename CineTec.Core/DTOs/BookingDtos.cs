using System;
using System.Collections.Generic;

namespace CineTec.Core.DTOs
{
    public class CrearReservaDto
    {
        public int FuncionId { get; set; }
        public List<int> ButacaIds { get; set; } = new();
    }

    public class EntradaDetalleDto
    {
        public int EntradaId { get; set; }
        public int ButacaId { get; set; }
        public string CodigoButaca { get; set; } = string.Empty;
        public string Fila { get; set; } = string.Empty;
        public int Columna { get; set; }
        public decimal Precio { get; set; }
        public string? CodigoUnico { get; set; }
        public string Estado { get; set; } = string.Empty;
    }

    public class PagoDetalleDto
    {
        public int PagoId { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public DateTime FechaPago { get; set; }
        public string Estado { get; set; } = string.Empty;
    }

    public class ReservaResponseDto
    {
        public int CompraId { get; set; }
        public int FuncionId { get; set; }
        public string PeliculaTitulo { get; set; } = string.Empty;
        public string SalaNombre { get; set; } = string.Empty;
        public DateTime FechaHoraInicio { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public DateTime FechaCompra { get; set; }
        public DateTime FechaExpiracion { get; set; }
        public double MinutosRestantes { get; set; }
        public List<EntradaDetalleDto> Entradas { get; set; } = new();
    }

    public class PagarReservaDto
    {
        public string MetodoPago { get; set; } = "Tarjeta"; // Tarjeta, QR, Transferencia
    }

    public class CompraDetalleDto
    {
        public int CompraId { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public string UsuarioEmail { get; set; } = string.Empty;
        public int FuncionId { get; set; }
        public string PeliculaTitulo { get; set; } = string.Empty;
        public string SalaNombre { get; set; } = string.Empty;
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCompra { get; set; }
        public DateTime FechaExpiracion { get; set; }
        public List<EntradaDetalleDto> Entradas { get; set; } = new();
        public List<PagoDetalleDto> Pagos { get; set; } = new();
    }
}
