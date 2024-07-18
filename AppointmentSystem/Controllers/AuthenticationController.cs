using AppointmentSystem.Business.Interface.IBusiness;
using AppointmentSystem.Entity.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController
    {
        private readonly IAuthenticationBusiness _authenticationBusiness;

        public AuthenticationController(IAuthenticationBusiness authenticationBusiness)
        {
            _authenticationBusiness = authenticationBusiness;
        }

        [HttpGet("login")]
        public async Task<UserTokenDTO> Login(string login, string password)
        {
            return await _authenticationBusiness.Login(login, password);
        }

        [HttpGet("refreshToken")]
        [ProducesResponseType(typeof(UserTokenDTO), StatusCodes.Status200OK)]
        [Authorize]
        public async Task<UserTokenDTO> RefreshToken()
        {
            return await _authenticationBusiness.RefreshToken();

        }
    }
}
