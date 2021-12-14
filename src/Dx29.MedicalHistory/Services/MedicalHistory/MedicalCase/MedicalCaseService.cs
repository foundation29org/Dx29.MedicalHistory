using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace Dx29.Services
{
    public partial class MedicalCaseService
    {
        public MedicalCaseService(MedicalCasesDatabase databaseService, CaseRecordService caseRecordService, AccountHashService accountHashService, ILogger<MedicalCaseService> logger)
        {
            DatabaseService = databaseService;
            CaseRecordService = caseRecordService;
            AccountHashService = accountHashService;
            Logger = logger;
        }

        public DatabaseService DatabaseService { get; }
        public CaseRecordService CaseRecordService { get; }
        public AccountHashService AccountHashService { get; }
        public ILogger<MedicalCaseService> Logger { get; }

        public Container MedicalCases { get; private set; }

        public async Task InitializeAsync()
        {
            MedicalCases = await GetOrCreateContainerAsync("medicalCases", "/userId");
            Logger.LogInformation("MedicalCaseService InitializeAsync");
        }

        private async Task<Container> GetOrCreateContainerAsync(string id, string partitionKeyPath, int? throughput = null)
        {
            var response = await DatabaseService.Database.CreateContainerIfNotExistsAsync(id, partitionKeyPath, throughput);
            if (response.StatusCode == HttpStatusCode.Created)
            {
                await CreateContainerIndexsAsync(response);
            }
            return response;
        }

        private async Task CreateContainerIndexsAsync(ContainerResponse response)
        {
            var indexingPolicy = response.Resource.IndexingPolicy;
            indexingPolicy.IndexingMode = IndexingMode.Consistent;

            // Add IncludePaths
            indexingPolicy.IncludedPaths.Clear();
            indexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/userId/?" });
            indexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/status/?" });
            indexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/createdOn/?" });
            indexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/updatedOn/?" });

            // Add ExcludedPaths
            indexingPolicy.ExcludedPaths.Add(new ExcludedPath { Path = "/*" });

            // Update
            await response.Container.ReplaceContainerAsync(response.Resource);
        }
    }
}
