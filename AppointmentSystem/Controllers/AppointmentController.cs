using AppointmentSystem.Business.Interface.IBusiness;
using AppointmentSystem.Entity.DTO;
using AppointmentSystem.Entity.Model;
using AppointmentSystem.Utils.Attributes;
using AppointmentSystem.Utils.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentBusiness _appointmentBusiness;
        private string _tokenJWT;
        public AppointmentController(IAppointmentBusiness appointmentBusiness)
        {
            _appointmentBusiness = appointmentBusiness;
        }

        [HttpPost("CreateAppointment")]
        [RequiredTransaction]
        [Authorize]
        public async Task<List<AppointmentDTO>> Post(AppointmentRegistrationModel newAppointment)
        {
            _tokenJWT = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            return await _appointmentBusiness.CreateAppointment(_tokenJWT, newAppointment);
        }

        [HttpGet("GetAppointmentById")]
        [Authorize]
        public async Task<AppointmentDTO> GetAppointmentById(int id)
        {
            _tokenJWT = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            return await _appointmentBusiness.GetAppointmentById(_tokenJWT, id);
        }

        [HttpGet("GetAllAppointments")]
        [Authorize(Roles = PermissionConstants.PROFESSIONAL)]
        public async Task<List<AppointmentDTO>> GetAllAppointments()
        {

            return await _appointmentBusiness.GetAppointments(null);
        }

        [HttpPost("FilterAppointments")]
        [Authorize]
        public async Task<List<AppointmentDTO>> FilterAppointments(AppointmentFilterModel filter)
        {
            return await _appointmentBusiness.GetAppointments(filter);
        }

        [HttpPut("UpdateAppointmentByPatient")]
        [RequiredTransaction]
        [Authorize(Roles = PermissionConstants.PATIENT)]
        public async Task<List<AppointmentDTO>> PutByPatient(int idAppointment, AppointmentUpdatePatientModel newAppointment)
        {
            _tokenJWT = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            return await _appointmentBusiness.UpdateAppointmentByPatient(_tokenJWT, idAppointment, newAppointment);
        }

        [HttpPut("UpdateAppointmentByProfessional")]
        [RequiredTransaction]
        [Authorize(Roles = PermissionConstants.PROFESSIONAL)]
        public async Task<List<AppointmentDTO>> PutByProfessional(int id, AppointmentUpdateProfessionalModel newAppointment)
        {
            return await _appointmentBusiness.UpdateAppointmentByProfessional(id, newAppointment);
        }


        [HttpDelete("DeleteAppointment")]
        [RequiredTransaction]
        [Authorize]
        public async Task<List<AppointmentDTO>> Delete(int id)
        {
            _tokenJWT = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            return await _appointmentBusiness.DeleteAppointment(_tokenJWT, id);
        }

        
    }
}
