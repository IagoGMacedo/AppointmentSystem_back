using AppointmentSystem.Business.Interface.IBusiness;
using AppointmentSystem.Entity.DTO;
using AppointmentSystem.Entity.Model;
using AppointmentSystem.Utils.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentBusiness _appointmentBusiness;
        public AppointmentController(IAppointmentBusiness appointmentBusiness)
        {
            _appointmentBusiness = appointmentBusiness;
        }

        [HttpPost("CreateAppointment")]
        [RequiredTransaction]
        public async Task<List<AppointmentDTO>> Post(AppointmentRegistrationModel newAppointment)
        {
            return await _appointmentBusiness.CreateAppointment(newAppointment);
        }

        [HttpGet("GetAllAppointments")]
        public async Task<List<AppointmentDTO>> GetAllAppointments()
        {

            return await _appointmentBusiness.GetAppointments(null);
        }

        [HttpPost("FilterAppointments")]
        public async Task<List<AppointmentDTO>> FilterAppointments(AppointmentFilterModel filter)
        {
            return await _appointmentBusiness.GetAppointments(filter);
        }

        [HttpPut("UpdateAppointmentByPatient")]
        [RequiredTransaction]
        public async Task<List<AppointmentDTO>> PutByPatient(int idAppointment, AppointmentUpdatePatientModel newAppointment)
        {
            return await _appointmentBusiness.UpdateAppointmentByPatient(idAppointment, newAppointment);
        }

        [HttpPut("UpdateAppointmentByProfessional")]
        [RequiredTransaction]
        public async Task<List<AppointmentDTO>> PutByProfessional(int id, AppointmentUpdateProfessionalModel newAppointment)
        {
            return await _appointmentBusiness.UpdateAppointmentByProfessional(id, newAppointment);
        }


        [HttpDelete("DeleteAppointment")]
        [RequiredTransaction]
        public async Task<List<AppointmentDTO>> Delete(int id)
        {
            return await _appointmentBusiness.DeleteAppointment(id);
        }

        
    }
}
