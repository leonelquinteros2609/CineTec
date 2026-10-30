using FluentValidation;
using CineTec.Core.DTOs;

namespace CineTec.Services.Validators
{
    public class CrearReservaDtoValidator : AbstractValidator<CrearReservaDto>
    {
        public CrearReservaDtoValidator()
        {
            RuleFor(x => x.FuncionId)
                .GreaterThan(0).WithMessage("El ID de la función es obligatorio.");

            // RN-03: Una compra puede incluir entre 1 y 8 entradas
            RuleFor(x => x.ButacaIds)
                .NotNull().WithMessage("Debe seleccionar al menos una butaca.")
                .Must(b => b != null && b.Count >= 1 && b.Count <= 8)
                .WithMessage("RN-03: Una compra debe incluir entre 1 y 8 entradas.");
        }
    }

    public class PagarReservaDtoValidator : AbstractValidator<PagarReservaDto>
    {
        public PagarReservaDtoValidator()
        {
            RuleFor(x => x.MetodoPago)
                .NotEmpty().WithMessage("El método de pago es obligatorio.")
                .MaximumLength(30).WithMessage("El método de pago no puede exceder 30 caracteres.");
        }
    }

    public class ValidarTicketDtoValidator : AbstractValidator<ValidarTicketDto>
    {
        public ValidarTicketDtoValidator()
        {
            RuleFor(x => x.CodigoUnico)
                .NotEmpty().WithMessage("El código único de la entrada es obligatorio.")
                .MaximumLength(64).WithMessage("El código único no puede exceder 64 caracteres.");
        }
    }
}
