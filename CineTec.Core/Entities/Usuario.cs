using System;
using System.Collections.Generic;

namespace CineTec.Core.Entities
{
    public class Usuario : BaseEntity
    {
        public string Nombre { get; set; } = null!;

        public string Email { get; set; } = null!;

        // Propiedad compatible con código previo
        public string Correo
        {
            get => Email;
            set => Email = value;
        }

        public string PasswordHash { get; set; } = null!;

        public string Rol { get; set; } = "Cliente";

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();
    }
}
