using AppointmentSystem.Entity.Enum;

namespace AppointmentSystem.Entity.Model
{
    public class UserFilterModel
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Login { get; set; }
        public ProfileEnum? Profile { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateOfCreation { get; set; }
    }
}
