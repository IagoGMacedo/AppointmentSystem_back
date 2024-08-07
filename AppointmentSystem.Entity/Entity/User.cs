﻿using AppointmentSystem.Entity.Enum;

namespace AppointmentSystem.Entity.Entity
{
    public class User : IdEntity<int>
    {
        public string Name { get; set; }
        public string Login { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public ProfileEnum Profile { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfCreation { get; set; }
        public List<Appointment> Appointments { get; set; }
        public User() { }
    }
}
