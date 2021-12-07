using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace Dx29.Services
{
    public partial class CaseRecordService
    {
        public CaseRecordService(CaseRecordsDatabase databaseService, RecordHashService recordHashService, ILogger<CaseRecordService> logger)
        {
            DatabaseService = databaseService;
            RecordHashService = recordHashService;
            Logger = logger;
        }

        public DatabaseService DatabaseService { get; }
        public RecordHashService RecordHashService { get; }
        public ILogger<CaseRecordService> Logger { get; }

        public Container CaseRecords { get; private set; }

        public async Task InitializeAsync()
        {
            CaseRecords = await GetOrCreateContainerAsync("caseRecords", "/userId");
            Logger.LogInformation("CaseRecordService InitializeAsync");
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
            indexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/type/?" });

            // Add ExcludedPaths
            indexingPolicy.ExcludedPaths.Add(new ExcludedPath { Path = "/*" });

            // Update
            await response.Container.ReplaceContainerAsync(response.Resource);
        }
    }
}
