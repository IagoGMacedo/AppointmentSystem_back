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
    public class AppointmentRegistrationValidator : AbstractValidator<AppointmentRegistrationModel>
    {
        public AppointmentRegistrationValidator()
        {
            RuleFor(appointment => appointment.UserId)
                .NotNull().WithMessage(string.Format(BusinessMessages.CampoObrigatorio, "Paciente"))
                .NotEmpty().WithMessage(string.Format(BusinessMessages.CampoInvalido, "Paciente"));

            RuleFor(appointment => appointment.AppointmentDate)
                .NotNull().WithMessage(string.Format(BusinessMessages.CampoObrigatorio, "Data do agendamento"));

            RuleFor(appointment => appointment.AppointmentTime)
                .NotNull().WithMessage(string.Format(BusinessMessages.CampoObrigatorio, "Horário do agendamento"));

        }
    }
}
