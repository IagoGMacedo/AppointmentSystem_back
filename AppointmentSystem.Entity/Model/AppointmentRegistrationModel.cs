namespace AppointmentSystem.Entity.Model
{
    public class AppointmentRegistrationModel
    {
        public int UserId { get; set; }
        public DateOnly? AppointmentDate { get; set; }
        public TimeSpan? AppointmentTime { get; set; }
    }
}
