using AppointmentSystem.Entity.Enum;

namespace AppointmentSystem.Entity.Entity
{
    public class Appointment : IdEntity<int>
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public StatusEnum Status { get; set; }
        public DateTime DateOfCreation { get; set; }
    }
}
