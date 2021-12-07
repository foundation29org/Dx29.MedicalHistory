using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace Dx29.Services
{
    public partial class ResourceGroupService
    {
        public ResourceGroupService(ResourceGroupsDatabase databaseService, ILogger<ResourceGroupService> logger)
        {
            DatabaseService = databaseService;
            Logger = logger;
        }

        public DatabaseService DatabaseService { get; }
        public ILogger<ResourceGroupService> Logger { get; }

        public Container ResourceGroups { get; private set; }

        public async Task InitializeAsync()
        {
            ResourceGroups = await GetOrCreateContainerAsync("resourceGroups", "/caseId");
            Logger.LogInformation("ResourceGroupService InitializeAsync");
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
            indexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/caseId/?" });
            indexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/name/?" });
            indexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/type/?" });
            indexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/createdOn/?" });
            indexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/updatedOn/?" });

            // Add ExcludedPaths
            indexingPolicy.ExcludedPaths.Add(new ExcludedPath { Path = "/*" });

            // Update
            await response.Container.ReplaceContainerAsync(response.Resource);
        }
    }
}
