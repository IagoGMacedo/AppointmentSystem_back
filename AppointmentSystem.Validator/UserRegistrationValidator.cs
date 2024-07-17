using AppointmentSystem.Entity.Entity;
using AppointmentSystem.Entity.Enum;
using AppointmentSystem.Entity.Model;
using AppointmentSystem.Utils.Messages;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSystem.Validator
{
    public class UserRegistrationValidator : AbstractValidator<UserRegistrationModel>
    {
        public UserRegistrationValidator()
        {
            RuleFor(user => user.Name)
                .NotNull().WithMessage(string.Format(BusinessMessages.CampoObrigatorio, "Nome"))
                .NotEmpty().WithMessage(string.Format(BusinessMessages.CampoInvalido, "Nome"));

            RuleFor(user => user.Login)
                .NotNull().WithMessage(string.Format(BusinessMessages.CampoObrigatorio, "Login"))
                .NotEmpty().WithMessage(string.Format(BusinessMessages.CampoInvalido, "Login"));

            RuleFor(t => t.Profile)
                .Must(profile => Enum.IsDefined(typeof(ProfileEnum), profile)).WithMessage(string.Format(BusinessMessages.CampoInvalido, "Perfil"));

            RuleFor(t => t.Password)
                .NotNull().WithMessage(string.Format(BusinessMessages.CampoObrigatorio, "Senha"))
                .NotEmpty().WithMessage(string.Format(BusinessMessages.CampoInvalido, "Senha"));

            RuleFor(user => user.DateOfBirth)
                .NotNull().WithMessage(string.Format(BusinessMessages.CampoObrigatorio, "Data de nascimento"))
                .NotEmpty().WithMessage(string.Format(BusinessMessages.CampoInvalido, "Data de nascimento"))
                ;

        }
    }
}
