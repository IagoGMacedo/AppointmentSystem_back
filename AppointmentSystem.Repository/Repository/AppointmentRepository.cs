using AppointmentSystem.Entity.DTO;
using AppointmentSystem.Entity.Model;
using AppointmentSystem.Repository.Interface.IRepository;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSystem.Repository.Repository
{
    public class AppointmentRepository : BaseRepository<Entity.Entity.Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(Context context) : base(context) { }

        public async Task<(bool IsDateAvailable, bool IsTimeAvailable)> CheckAvailability(DateOnly appointmentDate, TimeSpan appointmentTime)
        {
            // conta o número de agendamentos na data especificada
            var dateCount = await _context.Appointments
                .Where(appointment => appointment.AppointmentDate.Equals(appointmentDate))
                .CountAsync();

            // conta o número de agendamentos na data e horário especificados
            var dateTimeCount = await _context.Appointments
                .Where(appointment => appointment.AppointmentDate.Equals(appointmentDate)
                    && appointment.AppointmentTime.Equals(appointmentTime))
                .CountAsync();

            // verifica se a data está disponível (menos de 20 agendamentos no dia)
            bool isDateAvailable = dateCount < 20;

            // verifica se o horário está disponível (menos de 2 agendamentos na data e horário)
            bool isTimeAvailable = dateTimeCount < 2;

            return (isDateAvailable, isTimeAvailable);
        }

        public async Task<List<AppointmentDTO>> GetAllAppointments()
        {
            var query = _context.Appointments
                        .OrderBy(appointment => appointment.AppointmentDate)
                        .OrderBy(appointment => appointment.AppointmentTime)
                        .Select(appointment => new AppointmentDTO
                        {
                            Id = appointment.Id,
                            UserId = appointment.UserId,
                            AppointmentDate = appointment.AppointmentDate,
                            AppointmentTime = appointment.AppointmentTime,
                            Status = appointment.Status,
                            DateOfCreation = appointment.DateOfCreation
                        });

            return await query.ToListAsync();

        }

        public async Task<List<AppointmentDTO>> GetAppointments(AppointmentFilterModel filter)
        {
            var query = _context.Appointments.AsQueryable();

            if (filter.Id != null)
            {
                query = query.Where(a => a.Id == filter.Id);
            }

            if (filter.UserId != null)
            {
                query = query.Where(a => a.UserId == filter.UserId);
            }

            if (filter.AppointmentDate != null)
            {
                query = query.Where(a => a.AppointmentDate == filter.AppointmentDate);
            }

            if (filter.AppointmentTime != null)
            {
                query = query.Where(a => a.AppointmentTime == filter.AppointmentTime);
            }

            if (filter.Status != null)
            {
                query = query.Where(a => a.Status == filter.Status);
            }

            var result = await query.Select(appointment => new AppointmentDTO
            {
                Id = appointment.Id,
                UserId = appointment.UserId,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentTime = appointment.AppointmentTime,
                Status = appointment.Status,
                DateOfCreation = appointment.DateOfCreation
            }).ToListAsync();

            return result;

        }
    }
}
