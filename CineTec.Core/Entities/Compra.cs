using System;
using System.Collections.Generic;

namespace CineTec.Core.Entities
{
    public class Compra : BaseEntity
    {
        public int UsuarioId { get; set; }

        public int FuncionId { get; set; }

        public DateTime FechaCompra { get; set; } = DateTime.Now;

        public DateTime FechaExpiracion { get; set; }

        public decimal Total { get; set; }

        // Pendiente, Pagada, Cancelada, Expirada
        public string Estado { get; set; } = "Pendiente";

        public virtual Usuario Usuario { get; set; } = null!;

        public virtual Funcion Funcion { get; set; } = null!;

        public virtual ICollection<Entrada> Entradas { get; set; } = new List<Entrada>();

        public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    }
}
