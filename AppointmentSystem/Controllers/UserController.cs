using AppointmentSystem.Business.Interface.IBusiness;
using AppointmentSystem.Entity.DTO;
using AppointmentSystem.Entity.Model;
using AppointmentSystem.Utils.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserBusiness _userBusiness;

        public UserController(IUserBusiness userBusiness)
        {
            _userBusiness = userBusiness;
        }

        [HttpGet("GetAllUsers")]
        public async Task<List<UserDTO>> GetAllUsers()
        {
            return await _userBusiness.GetUsers(null);
        }

        [HttpGet("FilterUsers")]
        public async Task<List<UserDTO>> FilterUsers([FromQuery] UserFilterModel filter)
        {
            return await _userBusiness.GetUsers(filter);
        }

        [HttpPost("CreateUser")]
        [RequiredTransaction]
        public async Task<List<UserDTO>> Post(UserRegistrationModel newUser)
        {
            return await _userBusiness.CreateUser(newUser);
        }

        [HttpDelete("DeleteUser")]
        [RequiredTransaction]
        public async Task<List<UserDTO>> Delete(int id)
        {
            return await _userBusiness.DeleteUser(id);
        }

        [HttpPut("UpdateUser")]
        [RequiredTransaction]
        public async Task<List<UserDTO>> Put(int id, UserUpdateModel newUser)
        {
            return await _userBusiness.UpdateUser(id, newUser);
        }
    }
}
