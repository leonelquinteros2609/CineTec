using System.Collections.Generic;

namespace CineTec.Core.Entities
{
    public class Sala : BaseEntity
    {
        public string Nombre { get; set; } = null!;

        public int Filas { get; set; }

        public int Columnas { get; set; }

        public int Capacidad { get; set; }

        public virtual ICollection<Butaca> Butacas { get; set; } = new List<Butaca>();

        public virtual ICollection<Funcion> Funciones { get; set; } = new List<Funcion>();
    }
}
