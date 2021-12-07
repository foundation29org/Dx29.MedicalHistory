using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dx29.Services
{
    static public class ServiceConfiguration
    {
        static public void ConfigureAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            AddAccountHash(services, configuration);
            AddRecordHash(services, configuration);

            services.AddSingleton<CaseRecordsDatabase>();
            services.AddSingleton<MedicalCasesDatabase>();
            services.AddSingleton<ResourceGroupsDatabase>();

            services.AddSingleton<CaseRecordService>();
            services.AddSingleton<MedicalCaseService>();
            services.AddSingleton<ResourceGroupService>();

            services.AddSingleton<MedicalHistoryService>();
        }

        static public async Task InitializeAppServicesAsync(this IServiceProvider serviceProvider)
        {
            await serviceProvider.GetService<CaseRecordsDatabase>().InitializeAsync();
            await serviceProvider.GetService<MedicalCasesDatabase>().InitializeAsync();
            await serviceProvider.GetService<ResourceGroupsDatabase>().InitializeAsync();
            await serviceProvider.GetService<CaseRecordService>().InitializeAsync();
            await serviceProvider.GetService<MedicalCaseService>().InitializeAsync();
            await serviceProvider.GetService<ResourceGroupService>().InitializeAsync();
        }

        static public void AddAccountHash(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton((sp) =>
            {
                return new AccountHashService(configuration["Account:Key"], Int32.Parse(configuration["Account:Inx"]), 28);
            });
        }

        static public void AddRecordHash(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton((sp) =>
            {
                return new RecordHashService(configuration["Records:Key"], Int32.Parse(configuration["Records:Inx"]), 30);
            });
        }
    }
}
