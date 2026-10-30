using System;
using FluentValidation;
using CineTec.Core.DTOs;

namespace CineTec.Services.Validators
{
    public class CrearFuncionDtoValidator : AbstractValidator<CrearFuncionDto>
    {
        public CrearFuncionDtoValidator()
        {
            RuleFor(x => x.PeliculaId)
                .GreaterThan(0).WithMessage("El ID de la película es obligatorio.");

            RuleFor(x => x.SalaId)
                .GreaterThan(0).WithMessage("El ID de la sala es obligatorio.");

            RuleFor(x => x.FechaHoraInicio)
                .GreaterThan(DateTime.Now).WithMessage("La fecha y hora de la función debe ser en el futuro.");

            RuleFor(x => x.PrecioBase)
                .GreaterThan(0).WithMessage("El precio base debe ser mayor a cero.");
        }
    }

    public class ActualizarFuncionDtoValidator : AbstractValidator<ActualizarFuncionDto>
    {
        public ActualizarFuncionDtoValidator()
        {
            RuleFor(x => x.PeliculaId)
                .GreaterThan(0).WithMessage("El ID de la película es obligatorio.");

            RuleFor(x => x.SalaId)
                .GreaterThan(0).WithMessage("El ID de la sala es obligatorio.");

            RuleFor(x => x.FechaHoraInicio)
                .NotEmpty().WithMessage("La fecha y hora de inicio es obligatoria.");

            RuleFor(x => x.PrecioBase)
                .GreaterThan(0).WithMessage("El precio base debe ser mayor a cero.");
        }
    }
}
