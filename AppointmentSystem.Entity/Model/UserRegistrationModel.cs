using AppointmentSystem.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
