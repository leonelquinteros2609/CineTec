using FluentValidation;
using CineTec.Core.DTOs;

namespace CineTec.Services.Validators
{
    public class UsuarioDtoValidator : AbstractValidator<UsuarioDto>
    {
        public UsuarioDtoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres.");

            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.Email) || !string.IsNullOrWhiteSpace(x.Correo))
                .WithMessage("El correo electrónico es obligatorio.");

            RuleFor(x => x.GetEmail())
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.GetEmail()))
                .WithMessage("El formato del correo electrónico no es válido.")
                .MaximumLength(255).WithMessage("El correo no puede exceder 255 caracteres.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(4).WithMessage("La contraseña debe tener al menos 4 caracteres.");

            RuleFor(x => x.Rol)
                .Must(rol => string.IsNullOrEmpty(rol) || rol == "Cliente" || rol == "Taquillero" || rol == "Administrador")
                .WithMessage("El rol debe ser 'Cliente', 'Taquillero' o 'Administrador'.");
        }
    }
}
