using AppointmentSystem.Business.Business;
using AppointmentSystem.Business.Interface.IBusiness;
using AppointmentSystem.Entity.Entity;
using AppointmentSystem.Entity.Enum;
using AppointmentSystem.Entity.Model;
using AppointmentSystem.Repository.Interface.IRepository;
using AppointmentSystem.Repository.Repository;
using AppointmentSystem.Utils.Exceptions;
using AppointmentSystem.Utils.Messages;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSystem.UnitTests
{
    public class UserBusinessTest : UnitTestBase
    {
        private IUserBusiness _business;
        private IUserRepository _repository;
        private HMACSHA512 hmac;
        private JwtSecurityTokenHandler tokenHandler;

        [SetUp]
        public void SetUp()
        {
            _repository = new UserRepository(_context);

            RegisterObject(typeof(IUserRepository), _repository);

            Register<IUserBusiness, UserBusiness>();

            _business = GetService<IUserBusiness>();

            hmac = new HMACSHA512();

            tokenHandler = new JwtSecurityTokenHandler();
        }

        [TearDown]
        public void TearDown()
        {
            hmac?.Dispose();
        }

        [TestCase(ProfileEnum.Professional)]
        [TestCase(ProfileEnum.Patient)]
        public void CreateUser_Sucess(ProfileEnum profile)
        {
            var novoUsuario = new UserRegistrationModel
            {
                Login = $"{profile.ToString()}sucess",
                Name = "Nome",
                Profile = profile,
                DateOfBirth = DateTime.Now,
                Password = "123"
            };

            async Task action() => await _business.CreateUser(novoUsuario);

            Assert.DoesNotThrowAsync(action);
        }

        [TestCase(ProfileEnum.Professional)]
        [TestCase(ProfileEnum.Patient)]
        public void CreateUser_ExistentUser(ProfileEnum profile)
        {
            var existingUser = new User
            {
                Login = $"{profile.ToString()}existent",
                Name = "Nome",
                Profile = profile,
                DateOfBirth = DateTime.Now,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password")),
                PasswordSalt = hmac.Key,
            };

            var newUser = new UserRegistrationModel
            {
                Login = $"{profile.ToString()}existent",
                Name = "Nome",
                Profile = profile,
                DateOfBirth = DateTime.Now,
                Password = "123"
            };

            _context.Add(existingUser);
            _context.SaveChanges();

            async Task action() => await _business.CreateUser(newUser);

            var exception = Assert.ThrowsAsync<BusinessException>(action);

            Assert.IsTrue(exception.Message == string.Format(BusinessMessages.RegistroExistente, existingUser.Login));
        }

        [Test]
        public async Task DeleteUser_Success()
        {
            var user = new User
            {
                Name = "UserToDelete",
                Login = "deleteuser",
                DateOfBirth = DateTime.Now,
                Profile = ProfileEnum.Patient,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password")),
                PasswordSalt = hmac.Key
            };

            _context.Add(user);
            await _context.SaveChangesAsync();

            string testToken = GenerateTestToken(user);

            async Task action() => await _business.DeleteUser(testToken, user.Id);

            Assert.DoesNotThrowAsync(action);
            var deletedUser = await _repository.GetById(user.Id);
            Assert.IsNull(deletedUser);
        }

        [Test]
        public void DeleteUser_WithAppointments()
        {
            var user = new User
            {
                Name = "UserWithAppointments",
                Login = "appointmentuser",
                DateOfBirth = DateTime.Now,
                Profile = ProfileEnum.Patient,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password")),
                PasswordSalt = hmac.Key,
                Appointments = new List<Appointment>
                {
                    new Appointment { Status = StatusEnum.Agendado }
                }
            };

            _context.Add(user);
            _context.SaveChanges();

            string testToken = GenerateTestToken(user);

            async Task action() => await _business.DeleteUser(testToken, user.Id);

            var exception = Assert.ThrowsAsync<BusinessException>(action);
            Assert.IsTrue(exception.Message == BusinessMessages.ApagarUsuarioComAgendamento);
        }

        [Test]
        public void DeleteUser_NotFound_ThrowsException()
        {
            async Task action() => await _business.DeleteUser("", -1);

            var exception = Assert.ThrowsAsync<BusinessException>(action);
            Assert.IsTrue(string.Format(BusinessMessages.UsuarioNaoEncontrado, -1) == exception.Message);
        }

        [Test]
        public async Task UpdateUser_Success()
        {
            var user = new User
            {
                Name = "OldName",
                Login = "OldUser",
                DateOfBirth = DateTime.Now,
                Profile = ProfileEnum.Patient,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password")),
                PasswordSalt = hmac.Key
            };

            _context.Add(user);
            await _context.SaveChangesAsync();

            string testToken = GenerateTestToken(user);

            var updateUser = new UserUpdateModel
            {
                Name = "NewName",
                Login = "NewLogin",
                DateOfBirth = DateTime.Now.AddYears(-20),
                Password = "NewPassword"
            };

            async Task action() => await _business.UpdateUser(testToken, user.Id, updateUser);

            Assert.DoesNotThrowAsync(action);
            var userAfterUpdate = await _repository.GetById(user.Id);
            Assert.AreEqual(updateUser.Name, userAfterUpdate.Name);
            Assert.AreEqual(updateUser.DateOfBirth, userAfterUpdate.DateOfBirth);
            Assert.AreEqual(updateUser.Login, userAfterUpdate.Login);

            hmac = new HMACSHA512(userAfterUpdate.PasswordSalt);
            byte[] password = hmac.ComputeHash(Encoding.UTF8.GetBytes(updateUser.Password));

            Assert.IsTrue(password.SequenceEqual(userAfterUpdate.PasswordHash));
        }

        [Test]
        public async Task UpdateUser_AnotherUser()
        {
            var user1 = new User
            {
                Name = "User1",
                Login = "User1",
                DateOfBirth = DateTime.Now,
                Profile = ProfileEnum.Patient,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password")),
                PasswordSalt = hmac.Key
            };

            var user2 = new User
            {
                Name = "User2",
                Login = "User2",
                DateOfBirth = DateTime.Now,
                Profile = ProfileEnum.Patient,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password")),
                PasswordSalt = hmac.Key
            };

            _context.Add(user1);
            _context.Add(user2);
            await _context.SaveChangesAsync();

            string testToken = GenerateTestToken(user1);

            var updateUser = new UserUpdateModel
            {
                Name = "NewName",
                Login = "NewLogin",
                DateOfBirth = DateTime.Now.AddYears(-20),
                Password = "NewPassword"
            };

            async Task action() => await _business.UpdateUser(testToken, user2.Id, updateUser);

            var exception = Assert.ThrowsAsync<BusinessException>(action);

            Assert.IsTrue(exception.Message == BusinessMessages.ContaNaoPertenceAoUsuario);
        }

        private string GenerateTestToken(User user)
        {
            var expiration = DateTime.Now.AddMinutes(5);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Sid, user.Id.ToString()),
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Role, user.Profile.ToString()),
                new("login", user.Login),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var privateKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MinhaChaveSuperComplexaQueNinguemNuncaVaiConseguirDescobrir"));
            var credentials = new SigningCredentials(privateKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "https://github.com/IagoGMacedo",
                audience: "IagoGMacedo",
            claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            return tokenHandler.WriteToken(token);
        }


    }
}
