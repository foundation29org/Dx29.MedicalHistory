using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Dx29.Services;
using Dx29.Web.Services;

namespace Sample
{
    static public class ServiceManager
    {
        const string MEDICALHISTORY_ENDPOINT = "http://localhost:8201/api/v1/";

        static ServiceManager()
        {
            var services = new ServiceCollection();

            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();
            services.AddSingleton<IConfiguration>(configuration);

            services.AddLogging(configure => {
                configure.AddConsole();
                configure.SetMinimumLevel(LogLevel.Warning);
            });

            services.ConfigureAppServices(configuration);

            services.AddSingleton<MedicalHistoryClient>();
            services.AddHttpClient<MedicalHistoryClient>(http =>
            {
                http.BaseAddress = new Uri(MEDICALHISTORY_ENDPOINT);
            });

            ServiceProvider = services.BuildServiceProvider();
        }

        static public async Task InitializeAsync()
        {
            await ServiceProvider.InitializeAppServicesAsync();
        }

        static public ServiceProvider ServiceProvider { get; private set; }

        static public TService GetService<TService>() => ServiceProvider.GetService<TService>();
    }
}
