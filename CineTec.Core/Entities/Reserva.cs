namespace CineTec.Core.Entities;

public class Reserva
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public int FuncionId { get; set; }
    public DateTime FechaReserva { get; set; } = DateTime.Now;
    public decimal MontoTotal { get; set; }
    public string Estado { get; set; } = "Confirmada";

    public virtual Usuario Usuario { get; set; } = null!;
    public virtual Funcion Funcion { get; set; } = null!;
    public virtual ICollection<ReservaButaca> ReservaButacas { get; set; } = new List<ReservaButaca>();
}