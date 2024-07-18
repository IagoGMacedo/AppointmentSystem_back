namespace AppointmentSystem.Entity.Model
{
    public class AppointmentUpdatePatientModel
    {
        public DateOnly AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
    }
}
