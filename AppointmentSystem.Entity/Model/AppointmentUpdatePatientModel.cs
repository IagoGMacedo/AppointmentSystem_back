﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSystem.Entity.Model
{
    public class AppointmentUpdatePatientModel
    {
        public DateOnly AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
    }
}
