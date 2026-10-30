using FluentValidation;
using CineTec.Core.DTOs;

namespace CineTec.Services.Validators
{
    public class CrearPeliculaDtoValidator : AbstractValidator<CrearPeliculaDto>
    {
        public CrearPeliculaDtoValidator()
        {
            RuleFor(x => x.Titulo)
                .NotEmpty().WithMessage("El título es obligatorio.")
                .MaximumLength(150).WithMessage("El título no puede exceder 150 caracteres.");

            RuleFor(x => x.DuracionMin)
                .GreaterThan(0).WithMessage("La duración debe ser mayor a 0 minutos.");

            RuleFor(x => x.Clasificacion)
                .NotEmpty().WithMessage("La clasificación es obligatoria.")
                .MaximumLength(20).WithMessage("La clasificación no puede exceder 20 caracteres.");

            RuleFor(x => x.Genero)
                .NotEmpty().WithMessage("El género es obligatorio.")
                .MaximumLength(50).WithMessage("El género no puede exceder 50 caracteres.");
        }
    }

    public class ActualizarPeliculaDtoValidator : AbstractValidator<ActualizarPeliculaDto>
    {
        public ActualizarPeliculaDtoValidator()
        {
            RuleFor(x => x.Titulo)
                .NotEmpty().WithMessage("El título es obligatorio.")
                .MaximumLength(150).WithMessage("El título no puede exceder 150 caracteres.");

            RuleFor(x => x.DuracionMin)
                .GreaterThan(0).WithMessage("La duración debe ser mayor a 0 minutos.");

            RuleFor(x => x.Clasificacion)
                .NotEmpty().WithMessage("La clasificación es obligatoria.");

            RuleFor(x => x.Genero)
                .NotEmpty().WithMessage("El género es obligatorio.");
        }
    }
}
