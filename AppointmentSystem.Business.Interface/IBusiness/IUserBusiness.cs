using AppointmentSystem.Entity.DTO;
using AppointmentSystem.Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSystem.Business.Interface.IBusiness
{
    public interface IUserBusiness
    {
        Task<List<UserDTO>> CreateUser(UserRegistrationModel newUser);
        Task<List<UserDTO>> UpdateUser(string tokenJWT, int idUser, UserUpdateModel updateUser);
        Task<List<UserDTO>> DeleteUser(string tokenJWT, int idUser);
        Task<List<UserDTO>> GetUsers(UserFilterModel filter);
    }
}
