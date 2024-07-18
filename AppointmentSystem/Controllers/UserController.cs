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
        private string _tokenJWT;

        public UserController(IUserBusiness userBusiness)
        {
            _userBusiness = userBusiness;
        }

        [HttpPost("CreateUser")]
        [RequiredTransaction]
        public async Task<List<UserDTO>> Post(UserRegistrationModel newUser)
        {
            return await _userBusiness.CreateUser(newUser);
        }

        [HttpGet("GetAllUsers")]
        [Authorize]
        public async Task<List<UserDTO>> GetAllUsers()
        {
            return await _userBusiness.GetUsers(null);
        }

        [HttpGet("FilterUsers")]
        [Authorize]
        public async Task<List<UserDTO>> FilterUsers([FromQuery] UserFilterModel filter)
        {
            return await _userBusiness.GetUsers(filter);
        }

        [HttpPut("UpdateUser")]
        [RequiredTransaction]
        [Authorize]
        public async Task<List<UserDTO>> Put(int id, UserUpdateModel newUser)
        {
            _tokenJWT = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            return await _userBusiness.UpdateUser(_tokenJWT, id, newUser);
        }


        [HttpDelete("DeleteUser")]
        [RequiredTransaction]
        [Authorize]
        public async Task<List<UserDTO>> Delete(int id)
        {
            _tokenJWT = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            return await _userBusiness.DeleteUser(_tokenJWT, id);
        }
        
    }
}
