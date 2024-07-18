﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSystem.Entity.DTO
{
    public class UserTokenDTO
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }

        public UserTokenDTO(string token, string refreshToken)
        {
            Token = token;
            RefreshToken = refreshToken;
        }
    }
}
