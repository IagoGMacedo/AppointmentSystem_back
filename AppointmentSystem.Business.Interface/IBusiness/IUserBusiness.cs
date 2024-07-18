using AppointmentSystem.Entity.DTO;
using AppointmentSystem.Entity.Model;

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
