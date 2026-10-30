using System.Collections.Generic;

namespace CineTec.Core.Entities
{
    public class Butaca : BaseEntity
    {
        public int SalaId { get; set; }

        public string Fila { get; set; } = null!;

        public int Columna { get; set; }

        public string Codigo { get; set; } = null!;

        public virtual Sala Sala { get; set; } = null!;

        public virtual ICollection<Entrada> Entradas { get; set; } = new List<Entrada>();
    }
}
