using System;
using System.Collections.Generic;

namespace CineTec.Core.Entities;

public partial class Butaca
{
    public int Id { get; set; }

    public int SalaId { get; set; }

    public string Fila { get; set; } = null!;

    public int Columna { get; set; }

    public string Codigo { get; set; } = null!;

    public virtual Sala Sala { get; set; } = null!;
}