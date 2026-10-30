using System.Collections.Generic;

namespace CineTec.Core.Entities
{
    public class Pelicula : BaseEntity
    {
        public string Titulo { get; set; } = null!;

        public string? Sinopsis { get; set; }

        public int DuracionMin { get; set; }

        // Compatible con código previo
        public int DuracionMinutos
        {
            get => DuracionMin;
            set => DuracionMin = value;
        }

        public string Clasificacion { get; set; } = null!;

        public string Genero { get; set; } = null!;

        public string Estado { get; set; } = "Activa";

        public virtual ICollection<Funcion> Funciones { get; set; } = new List<Funcion>();
    }
}
