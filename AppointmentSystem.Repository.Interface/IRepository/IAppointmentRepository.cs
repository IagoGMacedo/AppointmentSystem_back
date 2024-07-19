using AppointmentSystem.Entity.DTO;
using AppointmentSystem.Entity.Model;

namespace AppointmentSystem.Repository.Interface.IRepository
{
    public interface IAppointmentRepository : IBaseRepository<Entity.Entity.Appointment>
    {
        Task<List<AppointmentDTO>> GetAppointments(AppointmentFilterModel filter);
        Task<AppointmentDTO> GetAppointment(AppointmentFilterModel filter);
        Task<(bool IsDateAvailable, bool IsTimeAvailable)> CheckAvailability(DateOnly appointmentDate, TimeSpan appointmentTime);
    }
}
