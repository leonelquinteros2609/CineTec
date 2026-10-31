using System;
using System.Collections.Generic;

namespace CineTec.Core.Entities
{
    public class Funcion : BaseEntity
    {
        public int PeliculaId { get; set; }

        public int SalaId { get; set; }

        public DateTime FechaHoraInicio { get; set; }

        public DateTime FechaHoraFin { get; set; }

        public decimal PrecioBase { get; set; }

        public string Estado { get; set; } = "Activa";

        public virtual Pelicula Pelicula { get; set; } = null!;

        public virtual Sala Sala { get; set; } = null!;

        public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();

        public virtual ICollection<Entrada> Entradas { get; set; } = new List<Entrada>();
    }
}
