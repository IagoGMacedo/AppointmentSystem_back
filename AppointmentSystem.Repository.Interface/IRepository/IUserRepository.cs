using AppointmentSystem.Entity.DTO;
using AppointmentSystem.Entity.Entity;
using AppointmentSystem.Entity.Filter;
using AppointmentSystem.Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSystem.Repository.Interface.IRepository
{
    public interface IUserRepository : IBaseRepository<Entity.Entity.User>
    {
        Task<List<UserDTO>> GetUsers(UserFilterModel filter);
        Task<List<UserDTO>> GetAllUsers();
        Task<User> GetUser(UserFilter filter);
    }
}
