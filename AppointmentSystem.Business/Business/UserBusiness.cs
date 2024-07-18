using AppointmentSystem.Business.Interface.IBusiness;
using AppointmentSystem.Entity.DTO;
using AppointmentSystem.Entity.Entity;
using AppointmentSystem.Entity.Filter;
using AppointmentSystem.Entity.Model;
using AppointmentSystem.Repository.Interface.IRepository;
using AppointmentSystem.Utils.Exceptions;
using AppointmentSystem.Utils.Messages;
using log4net;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSystem.Business.Business
{
    public class UserBusiness : IUserBusiness
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(UserBusiness));
        private readonly IUserRepository _userRepository;

        public UserBusiness(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserDTO>> CreateUser(UserRegistrationModel newUser)
        {
            var user = await _userRepository.GetUser(new UserFilter { Login = newUser.Login });

            if (user != null)
            {
                _log.InfoFormat(BusinessMessages.RegistroExistente, newUser.Login);
                throw new BusinessException(string.Format(BusinessMessages.RegistroExistente, newUser.Login));
            }

            user = BuildUser(newUser);
            await _userRepository.Create(user);
            _log.InfoFormat("O novo Usuário '{0}' foi inserido.", user.Name);
            return await _userRepository.GetAllUsers();
        }

        public async Task<List<UserDTO>> GetUsers(UserFilterModel filter)
        {
            if (filter == null)
            {
                return await _userRepository.GetAllUsers();
            }
            else
            {
                return await _userRepository.GetUsers(filter);
            }
        }

        public async Task<List<UserDTO>> UpdateUser(string tokenJWT, int idUser, UserUpdateModel updateUser)
        {
            var user = await _userRepository.GetById(idUser);
            if (user != null)
            {
                await CheckUserOwnsAccount(tokenJWT, user);

                var userLogin = await _userRepository.GetUser(new UserFilter { Login = updateUser.Login });
                if (userLogin != null && userLogin.Id != user.Id)
                {
                    _log.InfoFormat(BusinessMessages.RegistroExistente, updateUser.Login);
                    throw new BusinessException(string.Format(BusinessMessages.RegistroExistente, updateUser.Login));
                }
                user = BuildUser(user, updateUser);

                await _userRepository.Update(user);
                _log.InfoFormat("O usuário '{0}' foi atualizado", idUser);
            }
            else
            {
                _log.InfoFormat("O usuário '{0}' não existe na base.", idUser);
                throw new BusinessException(string.Format(BusinessMessages.UsuarioNaoEncontrado, idUser));
            }
            return await _userRepository.GetAllUsers();
        }

        public async Task<List<UserDTO>> DeleteUser(string tokenJWT, int idUser)
        {
            var user = await _userRepository.GetUser(new UserFilter { Id = idUser });
            if (user != null)
            {
                await CheckUserOwnsAccount(tokenJWT, user);
                if (user.Appointments != null && user.Appointments.Count > 0)
                {
                    _log.InfoFormat("O usuário de id '{0}' não pode ser apagado pois possui agendamentos", user.Id);
                    throw new BusinessException(BusinessMessages.ApagarUsuarioComAgendamento);
                }
                await _userRepository.Delete(user);
                _log.InfoFormat("O usuário '{0}' foi removido.", user.Name);
            }
            else
            {
                _log.InfoFormat("O usuário '{0}' não existe na base.", idUser);
                throw new BusinessException(string.Format(BusinessMessages.UsuarioNaoEncontrado, idUser));
            }
            return await _userRepository.GetAllUsers();
        }

        private async Task CheckUserOwnsAccount(string tokenJWT, User user)
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(tokenJWT);
            var userRole = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?.Value;

            if (userRole.ToUpper() == "PATIENT")
            {
                var userID = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid)?.Value;

                if (userID != null && userID != user.Id.ToString())
                {
                    _log.InfoFormat("O usuário de id '{0}' não pode interagir com a conta de id '{1}' ", userID, user.Id);
                    throw new BusinessException(BusinessMessages.ContaNaoPertenceAoUsuario);
                }
            }
        }

        private User BuildUser(UserRegistrationModel newUser)
        {
            var user = new User
            {
                Name = newUser.Name,
                DateOfBirth = newUser.DateOfBirth,
                Profile = newUser.Profile,
                Login = newUser.Login,
                DateOfCreation = DateTime.Now
            };

            return user;
        }

        private User BuildUser(User user, UserUpdateModel updateUser)
        {
            user.Name = updateUser.Name;
            user.Login = updateUser.Login;
            user.DateOfBirth = updateUser.DateOfBirth;

            return user;
        }
    }
}
