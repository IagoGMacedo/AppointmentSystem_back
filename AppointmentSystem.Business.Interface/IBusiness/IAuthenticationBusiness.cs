using AppointmentSystem.Entity.DTO;
using AppointmentSystem.Entity.Entity;

namespace AppointmentSystem.Business.Interface.IBusiness
{
    public interface IAuthenticationBusiness
    {
        Task<UserTokenDTO> Login(string login, string password);
        Task<UserTokenDTO> RefreshToken();
        string GenerateToken(User user);
    }
}
