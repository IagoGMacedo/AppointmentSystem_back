using AppointmentSystem.Entity.DTO;
using AppointmentSystem.Entity.Entity;
using AppointmentSystem.Entity.Filter;
using AppointmentSystem.Entity.Model;
using AppointmentSystem.Repository.Interface.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSystem.Repository.Repository
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(Context context) : base(context) { }

        public async Task<List<UserDTO>> GetAllUsers()
        {
            var query = EntitySet
               .OrderBy(user => user.Name)
               .Select(user => new UserDTO
               {
                   Id = user.Id,
                   Name = user.Name,
                   Profile = user.Profile,
                   DateOfBirth = user.DateOfBirth,
                   Appointments = user.Appointments.Select(a => new AppointmentDTO
                   {
                       Id = a.Id,
                       AppointmentDate = a.AppointmentDate,
                       AppointmentTime = a.AppointmentTime,
                       Status = a.Status,
                       DateOfCreation = a.DateOfCreation
                   }).ToList()
               });

            return await query.ToListAsync();
        }

        public Task<User> GetUser(UserFilter filter)
        {
            var query = EntitySet.AsQueryable();
            query = EntitySet.Include(e => e.Appointments);

            if (filter.Id.HasValue)
                query = query.Where(e => e.Id == filter.Id.Value);

            if (!string.IsNullOrEmpty(filter.Login))
                query = query.Where(e => e.Login == filter.Login);

            return query.FirstOrDefaultAsync();
        }

        public async Task<List<UserDTO>> GetUsers(UserFilterModel filter)
        {
            var query = _context.Users.AsQueryable();

            if (filter.Id != null)
            {
                query = query.Where(a => a.Id == filter.Id);
            }

            if (filter.Name != null)
            {
                query = query.Where(a => a.Name.ToLower().Contains(filter.Name.ToLower()));
            }

            if (filter.Login != null)
            {
                query = query.Where(a => a.Login.Equals(filter.Login));
            }

            if (filter.Profile != null)
            {
                query = query.Where(a => a.Profile.Equals(filter.Profile));
            }

            if (filter.DateOfBirth != null)
            {
                query = query.Where(a => a.DateOfBirth == filter.DateOfBirth);
            }

            if (filter.DateOfCreation != null)
            {
                query = query.Where(a => a.DateOfCreation == filter.DateOfCreation);
            }

            var result = await query.Select(user => new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Profile = user.Profile,
                DateOfBirth = user.DateOfBirth,
                Appointments = user.Appointments.Select(a => new AppointmentDTO
                {
                    Id = a.Id,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    Status = a.Status,
                    DateOfCreation = a.DateOfCreation
                }).ToList()
            }).ToListAsync();

            return result;
        }
    }
}
