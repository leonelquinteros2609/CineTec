using System;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CineTec.Core.Entities;

public partial class Pelicula
{
    public int Id { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Sinopsis { get; set; }

    public int DuracionMinutos { get; set; }

    public string Clasificacion { get; set; } = null!;

    public string Genero { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public virtual ICollection<Funcion> Funciones { get; set; } = new List<Funcion>();
}