using AppointmentSystem.Entity.DTO;
using AppointmentSystem.Entity.Entity;
using AppointmentSystem.Entity.Filter;
using AppointmentSystem.Entity.Model;

namespace AppointmentSystem.Repository.Interface.IRepository
{
    public interface IUserRepository : IBaseRepository<Entity.Entity.User>
    {
        Task<List<UserDTO>> GetUsers(UserFilterModel filter);
        Task<List<UserDTO>> GetAllUsers();
        Task<User> GetUser(UserFilter filter);
        Task<List<UserNameAndIdDTO>> GetUsersNamesAndIds();
    }
}
