using AppointmentSystem.Business.Interface.IBusiness;
using AppointmentSystem.Entity.DTO;
using AppointmentSystem.Entity.Entity;
using AppointmentSystem.Entity.Enum;
using AppointmentSystem.Entity.Filter;
using AppointmentSystem.Entity.Model;
using AppointmentSystem.Repository.Interface.IRepository;
using AppointmentSystem.Utils.Exceptions;
using AppointmentSystem.Utils.Messages;
using log4net;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Policy;

namespace AppointmentSystem.Business.Business
{
    public class AppointmentBusiness : IAppointmentBusiness
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(AppointmentBusiness));
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUserRepository _userRepository;
        public AppointmentBusiness(IAppointmentRepository appointmentRepository, IUserRepository userRepository)
        {
            _appointmentRepository = appointmentRepository;
            _userRepository = userRepository;
        }

        public async Task<List<AppointmentDTO>> CreateAppointment(string tokenJWT, AppointmentRegistrationModel newAppointment)
        {
            var user = await _userRepository.GetUser(new UserFilter { Id = newAppointment.UserId });

            if (user == null)
            {
                _log.InfoFormat("O usuário de id '{0}' não existe na base.", newAppointment.UserId);
                throw new BusinessException(string.Format(BusinessMessages.UsuarioNaoEncontrado, newAppointment.UserId));
            }

            // verifica se o paciente está criando um agendamento para si mesmo
            await CheckUserOwnsAppointment(tokenJWT, BuildAppointment(newAppointment, user));

            // verifica se o paciente já não possui um agendamento marcado
            if (user.Appointments.Any(appointment => appointment.Status == StatusEnum.Agendado))
            {
                _log.InfoFormat("O usuário de id '{0}' não pode criar um novo agendamento pois já possui um marcado", newAppointment.UserId);
                throw new BusinessException(string.Format(BusinessMessages.UsuarioJaPossuiAgendamento, newAppointment.UserId));
            }

            await CheckAppointmentAvailability(newAppointment.AppointmentDate.Value, newAppointment.AppointmentTime.Value);

            var appointment = BuildAppointment(newAppointment, user);
            await _appointmentRepository.Create(appointment);
            _log.InfoFormat("O novo Agendamento {0} {1} foi inserido.", appointment.AppointmentDate, appointment.AppointmentTime);
            return await _appointmentRepository.GetAppointments(null);
        }

        public async Task<List<AppointmentDTO>> GetAppointments(AppointmentFilterModel filter)
        {
            if (filter == null)
            {
                return await _appointmentRepository.GetAppointments(null);
            }
            else
            {
                return await _appointmentRepository.GetAppointments(filter);
            }
        }

        public async Task<AppointmentDTO> GetAppointmentById(string tokenJWT, int idAppointment)
        {
            var appointment = await _appointmentRepository.GetById(idAppointment);
            if(appointment == null)
            {
                _log.InfoFormat("O agendamento '{0}' não existe na base.", idAppointment);
                throw new BusinessException(string.Format(BusinessMessages.AgendamentoNaoEncontrado, idAppointment));
            }
            await CheckUserOwnsAppointment(tokenJWT, appointment);
            return await _appointmentRepository.GetAppointment(new AppointmentFilterModel { Id=idAppointment});
        }

        public async Task<List<AppointmentDTO>> UpdateAppointmentByPatient(string tokenJWT, int idAppointment, AppointmentUpdatePatientModel updateAppointment)
        {
            var appointment = await _appointmentRepository.GetById(idAppointment);
            if (appointment != null)
            {
                if(updateAppointment.Status != StatusEnum.Cancelado) { 
                    await CheckUserOwnsAppointment(tokenJWT, appointment);
                    if (appointment.AppointmentDate != updateAppointment.AppointmentDate || appointment.AppointmentTime != updateAppointment.AppointmentTime)
                        await CheckAppointmentAvailability(updateAppointment.AppointmentDate, updateAppointment.AppointmentTime);

                    appointment.AppointmentDate = updateAppointment.AppointmentDate;
                    appointment.AppointmentTime = updateAppointment.AppointmentTime;
                    appointment.DateOfCreation = DateTime.Now;
                }
                //só é possivel alterar para cancelado
                else 
                    appointment.Status = StatusEnum.Cancelado;
                
                await _appointmentRepository.Update(appointment);
                _log.InfoFormat("O agendamento '{0}' foi atualizado", idAppointment);
            }
            else
            {
                _log.InfoFormat("O agendamento '{0}' não existe na base.", idAppointment);
                throw new BusinessException(string.Format(BusinessMessages.AgendamentoNaoEncontrado, idAppointment));
            }
            return await _appointmentRepository.GetAppointments(null);
        }

        public async Task<List<AppointmentDTO>> UpdateAppointmentByProfessional(int idAppointment, AppointmentUpdateProfessionalModel updateAppointment)
        {
            var appointment = await _appointmentRepository.GetById(idAppointment);
            if (appointment != null)
            {
                if(updateAppointment.Status != StatusEnum.Cancelado && updateAppointment.Status != StatusEnum.Concluido)
                {
                    if (appointment.AppointmentDate != updateAppointment.AppointmentDate || appointment.AppointmentTime != updateAppointment.AppointmentTime)
                        await CheckAppointmentAvailability(updateAppointment.AppointmentDate, updateAppointment.AppointmentTime);
                    
                    appointment.AppointmentDate = updateAppointment.AppointmentDate;
                    appointment.AppointmentTime = updateAppointment.AppointmentTime;
                    appointment.UserId = updateAppointment.UserId;
                    appointment.DateOfCreation = DateTime.Now;
                }
                else
                    appointment.Status = updateAppointment.Status;
                
                await _appointmentRepository.Update(appointment);
                _log.InfoFormat("O agendamento '{0}' foi atualizado", idAppointment);
            }
            else
            {
                _log.InfoFormat("O agendamento '{0}' não existe na base.", idAppointment);
                throw new BusinessException(string.Format(BusinessMessages.AgendamentoNaoEncontrado, idAppointment));
            }
            return await _appointmentRepository.GetAppointments(null);
        }

        public async Task<List<AppointmentDTO>> DeleteAppointment(string tokenJWT, int idAppointment)
        {
            var appointment = await _appointmentRepository.GetById(idAppointment);
            if (appointment != null)
            {
                await CheckUserOwnsAppointment(tokenJWT, appointment);
                await _appointmentRepository.Delete(appointment);
                _log.InfoFormat("O agendamento {0} {1} foi removido.", appointment.AppointmentDate, appointment.AppointmentTime);
            }
            else
            {
                _log.InfoFormat("O agendamento '{0}' não existe na base.", idAppointment);
                throw new BusinessException(string.Format(BusinessMessages.AgendamentoNaoEncontrado, idAppointment));
            }
            return await _appointmentRepository.GetAppointments(null);
        }

        // verifica se, caso seja um paciente, ele está criando, alterando ou excluindo um agendamento que pertence a ele
        private async Task CheckUserOwnsAppointment(string tokenJWT, Appointment appointment)
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(tokenJWT);
            var userRole = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?.Value;

            if (userRole.ToUpper() == "PATIENT")
            {
                var userID = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid)?.Value;

                if (userID != null && userID != appointment.UserId.ToString())
                {
                    _log.InfoFormat("O usuário de id '{0}' não pode interagir com o agendamento de outro usuário", userID);
                    throw new BusinessException(BusinessMessages.AgendamentoNaoPertenceAoUsuario);
                }
            }
        }

        private async Task CheckAppointmentAvailability(DateOnly appointmentDate, TimeSpan appointmentTime)
        {
            if (appointmentTime.Minutes != 0 || appointmentTime.Seconds != 0)
            {
                _log.InfoFormat("O horário do agendamento não deve especificar minuto ou segundo");
                throw new BusinessException(BusinessMessages.HorarioInvalido);
            }

            if (appointmentTime.Hours < 7 || appointmentTime.Hours > 17)
            {
                _log.InfoFormat("Os agendamentos ocorrem de 07 às 17");
                throw new BusinessException(BusinessMessages.HorarioExpediente);
            }

            var availability = await _appointmentRepository.CheckAvailability(appointmentDate, appointmentTime);

            if (!availability.IsDateAvailable)
            {
                _log.InfoFormat("Não existem mais horários disponíveis para esse dia");
                throw new BusinessException(BusinessMessages.DiaIndisponivel);
            }

            if (!availability.IsTimeAvailable)
            {
                _log.InfoFormat("Esse horário não está disponível");
                throw new BusinessException(BusinessMessages.HorarioIndisponivel);
            }
        }

        private Appointment BuildAppointment(AppointmentRegistrationModel newAppointment, User user)
        {

            var appointment = new Appointment
            {
                UserId = newAppointment.UserId,
                DateOfCreation = DateTime.Now,
                AppointmentDate = newAppointment.AppointmentDate.Value,
                AppointmentTime = newAppointment.AppointmentTime.Value,
                Status = Entity.Enum.StatusEnum.Agendado,
            };

            return appointment;
        }

        
    }
}
