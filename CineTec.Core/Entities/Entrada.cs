namespace CineTec.Core.Entities
{
    public class Entrada : BaseEntity
    {
        public int CompraId { get; set; }

        public int FuncionId { get; set; }

        public int ButacaId { get; set; }

        public decimal Precio { get; set; }

        // Código único de 64 caracteres generado al momento del pago (RN-09)
        public string? CodigoUnico { get; set; }

        // Reservada, Pagada, Usada, Cancelada
        public string Estado { get; set; } = "Reservada";

        public virtual Compra Compra { get; set; } = null!;

        public virtual Funcion Funcion { get; set; } = null!;

        public virtual Butaca Butaca { get; set; } = null!;
    }
}
