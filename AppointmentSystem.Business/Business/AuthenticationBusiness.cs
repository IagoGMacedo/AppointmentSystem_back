using AppointmentSystem.Business.Interface.IBusiness;
using AppointmentSystem.Entity.DTO;
using AppointmentSystem.Entity.Entity;
using AppointmentSystem.Entity.Filter;
using AppointmentSystem.Repository.Interface.IRepository;
using AppointmentSystem.Utils.Configurations;
using AppointmentSystem.Utils.Extensions;
using AppointmentSystem.Utils.Messages;
using AppointmentSystem.Utils.UserContext;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AppointmentSystem.Business.Business
{
    public class AuthenticationBusiness : IAuthenticationBusiness
    {
        private readonly IUserRepository _userRepository;
        private readonly AuthenticationConfig _authenticationConfig;
        private readonly IUserContext _userContext;
        public AuthenticationBusiness(IUserRepository usuarioRepositorio,
                                   IOptionsMonitor<AuthenticationConfig> autenticacaoConfig,
                                   IUserContext usuarioContexto)
        {
            _userRepository = usuarioRepositorio;
            _authenticationConfig = autenticacaoConfig.CurrentValue;
            _userContext = usuarioContexto;
        }

        public async Task<UserTokenDTO> Login(string login, string password)
        {
            var userValid = await Authenticate(login, password);
            var user = await _userRepository.GetUser(new UserFilter { Login = login });
            string token;
            string refreshToken;

            if (userValid && user != null)
            {
                token = GenerateToken(user);
                refreshToken = GenerateRefreshToken(user);
            }
            else
                throw new UnauthorizedAccessException(BusinessMessages.UsuarioSenhaInvalida);

            return new UserTokenDTO(token, refreshToken);
        }

        public async Task<UserTokenDTO> RefreshToken()
        {
            var login = _userContext.Login();
            var usuario = await _userRepository.GetUser(new UserFilter { Login = login });
            string token;
            string refreshToken;

            if (usuario != null)
            {
                token = GenerateToken(usuario);
                refreshToken = GenerateRefreshToken(usuario);
            }
            else
                throw new UnauthorizedAccessException();

            return new UserTokenDTO(token, refreshToken);
        }

        public async Task<bool> Authenticate(string login, string password)
        {
            var user = await _userRepository.GetUser(new UserFilter { Login = login });

            if (user == null)
                return false;

            using var hmac = new HMACSHA512(user.PasswordSalt);

            return hmac.ComputeHash(Encoding.UTF8.GetBytes(password))
                       .SequenceEqual(user.PasswordHash);
        }

        public string GenerateToken(User user)
        {
            var expiration = DateTime.Now.AddMinutes(_authenticationConfig.AccessTokenExpiration);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Sid, user.Id.ToString()),
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Role, user.Profile.ToString()),
                new("login", user.Login),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var privateKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationConfig.SecretKey));
            var credentials = new SigningCredentials(privateKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _authenticationConfig.Issuer,
                audience: _authenticationConfig.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken(User user)
        {
            var expiration = DateTime.Now.AddMinutes(_authenticationConfig.RefreshTokenExpiration);

            var claims = new List<Claim>
            {
                new("login", user.Login)
            };

            var privateKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationConfig.SecretKey));
            var credentials = new SigningCredentials(privateKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _authenticationConfig.Issuer,
                audience: _authenticationConfig.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
