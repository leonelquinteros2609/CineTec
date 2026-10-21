using System;
using System.Collections.Generic;

namespace CineTec.Core.Entities;

public partial class Funcion
{
    public int Id { get; set; }

    public int PeliculaId { get; set; }

    public int SalaId { get; set; }

    public DateTime FechaHoraInicio { get; set; }

    public DateTime FechaHoraFin { get; set; }

    public decimal PrecioBase { get; set; }

    public string Estado { get; set; } = null!;

    public virtual Pelicula Pelicula { get; set; } = null!;

    public virtual Sala Sala { get; set; } = null!;
}