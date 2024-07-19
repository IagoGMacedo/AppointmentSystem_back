using AppointmentSystem.Entity.Entity;
using AppointmentSystem.Entity.Enum;

namespace AppointmentSystem.Entity.DTO
{
    public class AppointmentDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public StatusEnum Status { get; set; }
        public DateTime DateOfCreation { get; set; }
    }
}
