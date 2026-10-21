using System;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CineTec.Core.Entities;

public partial class Sala
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public int Filas { get; set; }

    public int Columnas { get; set; }

    public int Capacidad { get; set; }

    public virtual ICollection<Butaca> Butacas { get; set; } = new List<Butaca>();

    public virtual ICollection<Funcion> Funciones { get; set; } = new List<Funcion>();
}