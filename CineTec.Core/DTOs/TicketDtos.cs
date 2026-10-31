using System;

namespace CineTec.Core.DTOs
{
    public class ValidarTicketDto
    {
        public string CodigoUnico { get; set; } = string.Empty;
    }

    public class TicketValidadoDto
    {
        public bool Valida { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public int EntradaId { get; set; }
        public string CodigoUnico { get; set; } = string.Empty;
        public string Pelicula { get; set; } = string.Empty;
        public string Sala { get; set; } = string.Empty;
        public string Butaca { get; set; } = string.Empty;
        public DateTime FechaHoraFuncion { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
