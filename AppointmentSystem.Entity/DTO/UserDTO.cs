using AppointmentSystem.Entity.Enum;

namespace AppointmentSystem.Entity.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public DateTime DateOfBirth { get; set; }

        public ProfileEnum Profile { get; set; }

        public List<AppointmentDTO> Appointments { get; set; }
    }
}
