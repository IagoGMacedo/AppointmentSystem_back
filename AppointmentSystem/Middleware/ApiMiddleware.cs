
using AppointmentSystem.Repository.Interface.IRepository;
using AppointmentSystem.Utils.Exceptions;
using AppointmentSystem.Utils.Messages;
using AppointmentSystem.Utils.Responses;
using log4net;
using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;

namespace AppointmentSystem.Api.Middleware
{
    public class ApiMiddleware : IMiddleware
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ApiMiddleware));
        private readonly ITransactionManager _transactionManager;

        public ApiMiddleware(ITransactionManager transactionManager)
        {
            _transactionManager = transactionManager;
        }

        public async System.Threading.Tasks.Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();

            //transacao obrigatoria
            var mandatoryTransaction = context.Features.Get<IEndpointFeature>()?.Endpoint?.Metadata.GetMetadata<RequiredTransactionAttribute>();
            try
            {
                if (mandatoryTransaction != null)
                {
                    await _transactionManager.BeginTransactionAsync(mandatoryTransaction.IsolationLevel);

                    await next.Invoke(context);

                    await _transactionManager.CommitTransactionAsync();
                }
                else
                {
                    await next.Invoke(context);
                }

                stopwatch.Stop();
                _log.InfoFormat("Serviço executado com sucesso: {0} {1} [{2} ms]", context.Request.Method, context.Request.Path, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                if (mandatoryTransaction != null)
                    await _transactionManager.RollbackTransactionAsync();

                stopwatch.Stop();
                _log.Error($"Erro no serviço: {context.Request.Path} / Mensagem: {ex.Message} [{stopwatch.ElapsedMilliseconds}]", ex);
                await HandleException(context, ex);
            }
        }


        private static async System.Threading.Tasks.Task HandleException(HttpContext context, Exception exception)
        {
            var response = context.Response;

            response.ContentType = "application/json";

            await response.WriteAsync(JsonConvert.SerializeObject(new DefaultResponse(HttpStatusCode.InternalServerError, GetMessages(exception))));
        }

        private static List<string> GetMessages(Exception exception)
        {
            var messages = new List<string>();

            switch (exception)
            {
                case BusinessException or UnauthorizedAccessException:
                    messages.Add(exception.Message);
                    break;
                case BusinessListException:
                    messages = ((BusinessListException)exception).Messages;
                    break;
                default:
                    messages.Add(InfraMessages.ErroInesperado);
                    break;
            }

            return messages;
        }
    }
}
