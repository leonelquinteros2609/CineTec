using System;

namespace CineTec.Core.Entities
{
    public class Pago : BaseEntity
    {
        public int CompraId { get; set; }

        public decimal Monto { get; set; }

        // Ej: Tarjeta, QR, Transferencia
        public string MetodoPago { get; set; } = null!;

        public DateTime FechaPago { get; set; } = DateTime.Now;

        public string Estado { get; set; } = "Completado";

        public virtual Compra Compra { get; set; } = null!;
    }
}
