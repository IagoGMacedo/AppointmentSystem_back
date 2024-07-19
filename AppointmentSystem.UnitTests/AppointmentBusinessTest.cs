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
    public class AppointmentBusinessTest : UnitTestBase
    {

        private IAppointmentBusiness _appointmentBusiness;
        private IAppointmentRepository _appointmentRepository;

        private IUserBusiness _userBusiness;
        private IUserRepository _userRepository;


        private HMACSHA512 hmac;

        private JwtSecurityTokenHandler tokenHandler;


        [SetUp]
        public void SetUp()
        {
            _userRepository = new UserRepository(_context);
            _appointmentRepository = new AppointmentRepository(_context);

            RegisterObject(typeof(IUserRepository), _userRepository);
            RegisterObject(typeof(IAppointmentRepository), _appointmentRepository);


            Register<IUserBusiness, UserBusiness>();
            Register<IAppointmentBusiness, AppointmentBusiness>();


            _userBusiness = GetService<IUserBusiness>();
            _appointmentBusiness = GetService<IAppointmentBusiness>();

            hmac = new HMACSHA512();
            tokenHandler = new JwtSecurityTokenHandler();
        }

        [TearDown]
        public void TearDown()
        {
            hmac?.Dispose();
        }

        [Test]
        public async Task CreateAppointment_Success()
        {
            var user = new User
            {
                Login = "user1",
                Name = "user1",
                Profile = ProfileEnum.Patient,
                DateOfBirth = DateTime.Now,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password")),
                PasswordSalt = hmac.Key,
            };

            _context.Add(user);
            _context.SaveChanges();

            var newAppointment = new AppointmentRegistrationModel
            {
                UserId = user.Id,
                AppointmentDate = DateOnly.FromDateTime(DateTime.Now),
                AppointmentTime = new TimeSpan(10, 0, 0)
            };

            string testToken = GenerateTestToken(user);


            async Task action() => await _appointmentBusiness.CreateAppointment(testToken, newAppointment);

            Assert.DoesNotThrowAsync(action);
        }

        [Test]
        public async Task CreateAppointment_UnavailableTime()
        {
            var appointment1 = new Appointment
            {
                AppointmentDate = DateOnly.FromDateTime(DateTime.Now),
                AppointmentTime = new TimeSpan(10, 0, 0),
                DateOfCreation = DateTime.Now,
                Status = StatusEnum.Agendado,
            };

            var appointment2 = new Appointment
            {
                AppointmentDate = DateOnly.FromDateTime(DateTime.Now),
                AppointmentTime = new TimeSpan(10, 0, 0),
                DateOfCreation = DateTime.Now,
                Status = StatusEnum.Agendado,
            };

            var user = new User
            {
                Login = "user5",
                Name = "user5",
                Profile = ProfileEnum.Patient,
                DateOfBirth = DateTime.Now,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password")),
                PasswordSalt = hmac.Key,
            };

            _context.Add(appointment1);
            _context.Add(appointment2);
            _context.Add(user);
            _context.SaveChanges();

            string testToken = GenerateTestToken(user);

            var appointment = new AppointmentRegistrationModel
            {
                UserId = user.Id,
                AppointmentDate = DateOnly.FromDateTime(DateTime.Now),
                AppointmentTime = new TimeSpan(10, 0, 0)
            };


            async Task action() => await _appointmentBusiness.CreateAppointment(testToken, appointment);

            var exception = Assert.ThrowsAsync<BusinessException>(action);

            Assert.IsTrue(exception.Message == BusinessMessages.HorarioIndisponivel);
        }

        [Test]
        public async Task CreateAppointment_UnavailableDay()
        {
            var appointmentDate = DateOnly.FromDateTime(DateTime.Now.AddDays(6));

            for (int hour = 7; hour <= 17; hour++)
            {
                for (int i = 0; i < 2; i++)
                {
                    var appointment = new Appointment
                    {
                        AppointmentDate = appointmentDate,
                        AppointmentTime = new TimeSpan(hour, 0, 0),
                        DateOfCreation = DateTime.Now,
                        Status = StatusEnum.Agendado,
                    };
                    _context.Add(appointment);
                }
            }

            var user = new User
            {
                Login = "testuser",
                Name = "Test User",
                Profile = ProfileEnum.Patient,
                DateOfBirth = DateTime.Now.AddYears(-30),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password")),
                PasswordSalt = hmac.Key,
            };

            _context.Add(user);
            _context.SaveChanges();

            var newAppointment = new AppointmentRegistrationModel
            {
                UserId = user.Id,
                AppointmentDate = appointmentDate,
                AppointmentTime = new TimeSpan(10, 0, 0)
            };

            string testToken = GenerateTestToken(user);

            async Task action() => await _appointmentBusiness.CreateAppointment(testToken, newAppointment);

            var exception = Assert.ThrowsAsync<BusinessException>(action);

            Assert.IsTrue(exception.Message == BusinessMessages.DiaIndisponivel);
        }

        [Test]
        public async Task CreateAppointment_HasAppointment()
        {
            var user = new User
            {
                Login = "user2",
                Name = "user2",
                Profile = ProfileEnum.Patient,
                DateOfBirth = DateTime.Now,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password")),
                PasswordSalt = hmac.Key,
            };

            _context.Add(user);

            var firstAppointment = new Appointment
            {
                UserId = user.Id,
                AppointmentDate = DateOnly.FromDateTime(DateTime.Now),
                AppointmentTime = new TimeSpan(10, 0, 0),
                DateOfCreation = DateTime.Now,
                Status = StatusEnum.Agendado,
            };

            _context.Add(firstAppointment);
            _context.SaveChanges();


            var secondAppointment = new AppointmentRegistrationModel
            {
                UserId = user.Id,
                AppointmentDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10)),
                AppointmentTime = new TimeSpan(14, 0, 0)
            };


            string testToken = GenerateTestToken(user);

            async Task action() => await _appointmentBusiness.CreateAppointment(testToken, secondAppointment);
            var exception = Assert.ThrowsAsync<BusinessException>(action);

            Assert.IsTrue(exception.Message == string.Format(BusinessMessages.UsuarioJaPossuiAgendamento, user.Id));
        }

        [Test]
        public async Task CreateAppointment_ForAnotherUser()
        {
            var user1 = new User
            {
                Login = "user6",
                Name = "user6",
                Profile = ProfileEnum.Patient,
                DateOfBirth = DateTime.Now,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password")),
                PasswordSalt = hmac.Key,
            };

            var user2 = new User
            {
                Login = "user7",
                Name = "user7",
                Profile = ProfileEnum.Patient,
                DateOfBirth = DateTime.Now,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password")),
                PasswordSalt = hmac.Key,
            };

            _context.Add(user1);
            _context.Add(user2);
            _context.SaveChanges();

            var appointment = new AppointmentRegistrationModel
            {
                UserId = user2.Id,
                AppointmentDate = DateOnly.FromDateTime(DateTime.Now),
                AppointmentTime = new TimeSpan(10, 0, 0)
            };


            string testToken = GenerateTestToken(user1);

            async Task action() => await _appointmentBusiness.CreateAppointment(testToken, appointment);
            var exception = Assert.ThrowsAsync<BusinessException>(action);

            Assert.IsTrue(exception.Message == BusinessMessages.AgendamentoNaoPertenceAoUsuario);
        }

        [Test]
        public async Task UpdateAppointmentPatient_Success()
        {
            var user = new User
            {
                Login = "user3",
                Name = "user3",
                Profile = ProfileEnum.Patient,
                DateOfBirth = DateTime.Now,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password")),
                PasswordSalt = hmac.Key,
            };


            var appointment = new Appointment
            {
                AppointmentDate = DateOnly.FromDateTime(DateTime.Now),
                AppointmentTime = new TimeSpan(10, 0, 0),
                DateOfCreation = DateTime.Now,
                Status = StatusEnum.Agendado,
            };

            string testToken = GenerateTestToken(user);

            _context.Add(user);
            _context.Add(appointment);
            await _context.SaveChangesAsync();

            var updateModel = new AppointmentUpdatePatientModel
            {
                AppointmentDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                AppointmentTime = new TimeSpan(11, 0, 0)
            };

            async Task action() => await _appointmentBusiness.UpdateAppointmentByPatient(testToken, appointment.Id, updateModel);

            Assert.DoesNotThrowAsync(action);
            var updatedAppointment = await _appointmentRepository.GetById(appointment.Id);
            Assert.AreEqual(updateModel.AppointmentDate, updatedAppointment.AppointmentDate);
            Assert.AreEqual(updateModel.AppointmentTime, updatedAppointment.AppointmentTime);
        }

        [Test]
        public async Task UpdateAppointmentProfessional_Success()
        {
            var user = new User
            {
                Login = "user1",
                Name = "user1",
                Profile = ProfileEnum.Patient,
                DateOfBirth = DateTime.Now,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password")),
                PasswordSalt = hmac.Key,
            };

            _context.Add(user);

            var appointment = new Appointment
            {
                AppointmentDate = DateOnly.FromDateTime(DateTime.Now),
                AppointmentTime = new TimeSpan(10, 0, 0),
                DateOfCreation = DateTime.Now,
                Status = StatusEnum.Agendado,
                UserId = user.Id
            };

            _context.Add(appointment);
            await _context.SaveChangesAsync();

            var updateModel = new AppointmentUpdateProfessionalModel
            {
                UserId = user.Id,
                AppointmentDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                AppointmentTime = new TimeSpan(10, 0, 0),
                Status = StatusEnum.Agendado
            };

            async Task action() => await _appointmentBusiness.UpdateAppointmentByProfessional(appointment.Id, updateModel);

            Assert.DoesNotThrowAsync(action);

            var updatedAppointment = await _appointmentRepository.GetById(appointment.Id);
            Assert.AreEqual(updateModel.AppointmentDate, updatedAppointment.AppointmentDate);
            Assert.AreEqual(updateModel.AppointmentTime, updatedAppointment.AppointmentTime);
            Assert.AreEqual(updateModel.Status, updatedAppointment.Status);
            Assert.AreEqual(updateModel.UserId, updatedAppointment.UserId);
        }

        [Test]
        public async Task DeleteAppointment_Success()
        {
            var user = new User
            {
                Login = "user4",
                Name = "user4",
                Profile = ProfileEnum.Patient,
                DateOfBirth = DateTime.Now,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password")),
                PasswordSalt = hmac.Key,
            };



            _context.Add(user);

            var appointment = new Appointment
            {
                UserId = user.Id,
                AppointmentDate = DateOnly.FromDateTime(DateTime.Now),
                AppointmentTime = new TimeSpan(10, 0, 0),
                DateOfCreation = DateTime.Now,
                Status = StatusEnum.Agendado,
            };

            _context.Add(appointment);
            await _context.SaveChangesAsync();

            string testToken = GenerateTestToken(user);

            async Task action() => await _appointmentBusiness.DeleteAppointment(testToken, appointment.Id);

            Assert.DoesNotThrowAsync(action);

            var deletedAppointment = await _appointmentRepository.GetById(appointment.Id);
            Assert.IsNull(deletedAppointment);
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
