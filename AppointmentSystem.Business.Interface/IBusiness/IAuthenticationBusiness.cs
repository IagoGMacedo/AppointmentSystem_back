using AppointmentSystem.Entity.DTO;
using AppointmentSystem.Entity.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSystem.Business.Interface.IBusiness
{
    public interface IAuthenticationBusiness
    {
        Task<UserTokenDTO> Login(string login, string password);
        Task<UserTokenDTO> RefreshToken();
        string GenerateToken(User user);
    }
}
