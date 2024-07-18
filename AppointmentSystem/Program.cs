using Microsoft.AspNetCore;
using System.Reflection;
using AppointmentSystem.Api;
using log4net;
using log4net.Config;


public static class Program
{
    private static readonly ILog _log = LogManager.GetLogger(typeof(Program));

    public static void Main(string[] args)
    {
        try
        {
            var logRepository = LogManager.GetRepository(Assembly.GetCallingAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            _log.Info("Iniciando a API");
            var webhost = WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();

            webhost.Build().Run();

        }
        catch (Exception ex)
        {
            _log.Fatal("Erro fatal", ex);
            throw;
        }
    }
}