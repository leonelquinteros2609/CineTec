namespace CineTec.Core.Entities;

public class ReservaButaca
{
    public int Id { get; set; }
    public int ReservaId { get; set; }
    public int ButacaId { get; set; }

    public virtual Reserva Reserva { get; set; } = null!;
    public virtual Butaca Butaca { get; set; } = null!;
}