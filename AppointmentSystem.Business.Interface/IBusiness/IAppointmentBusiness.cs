using AppointmentSystem.Entity.DTO;
using AppointmentSystem.Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSystem.Business.Interface.IBusiness
{
    public interface IAppointmentBusiness
    {
        Task<List<AppointmentDTO>> CreateAppointment(AppointmentRegistrationModel newAppointment);
        Task<List<AppointmentDTO>> UpdateAppointmentByPatient(int idAppointment, AppointmentUpdatePatientModel updateAppointment);
        Task<List<AppointmentDTO>> UpdateAppointmentByProfessional(int idAppointment, AppointmentUpdateProfessionalModel updateAppointment);
        Task<List<AppointmentDTO>> DeleteAppointment(int idAppointment);
        Task<List<AppointmentDTO>> GetAppointments(AppointmentFilterModel filter);
    }
}
