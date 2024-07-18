using AppointmentSystem.Entity.Enum;

namespace AppointmentSystem.Entity.Model
{
    public class UserRegistrationModel
    {
        public string Name { get; set; }
        public string Login { get; set; }
        public ProfileEnum Profile { get; set; }
        public string Password { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
