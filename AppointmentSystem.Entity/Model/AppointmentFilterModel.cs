using AppointmentSystem.Entity.Enum;

namespace AppointmentSystem.Entity.Model
{
    public class AppointmentFilterModel
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public DateOnly? AppointmentDate { get; set; }
        public TimeSpan? AppointmentTime { get; set; }
        public StatusEnum? Status { get; set; }
    }
}
