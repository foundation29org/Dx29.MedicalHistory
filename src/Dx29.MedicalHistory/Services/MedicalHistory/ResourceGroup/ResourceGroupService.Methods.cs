using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

using Dx29.Data;

namespace Dx29.Services
{
    partial class ResourceGroupService
    {
        //
        //  Get
        //
        public async Task<ResourceGroup> GetResourceGroupByIdAsync(string caseId, string groupId)
        {
            try
            {
                var response = await ResourceGroups.ReadItemAsync<ResourceGroup>(groupId, new PartitionKey(caseId));
                Logger.LogInformation("GetResourceGroupByIdAsync RUs {RUs}", response.RequestCharge);
                return response;
            }
            catch (CosmosException ex)
            {
                if (ex.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    Logger.LogError("GetResourceGroupByIdAsync. {exception}", ex);
                    throw;
                }
                Logger.LogWarning("GetResourceGroupByIdAsync. Not found.");
            }
            catch (Exception ex)
            {
                Logger.LogError("GetResourceGroupByIdAsync. {exception}", ex);
            }
            return null;
        }

        public async Task<ResourceGroup> GetResourceGroupByTypeNameAsync(string caseId, ResourceGroupType type, string name)
        {
            var records = ResourceGroups.GetRecordsAsync<ResourceGroup>(Logger, r => r.CaseId == caseId && r.Name.ToLower() == name.ToLower() && r.Type == type.ToString());
            return await records.FirstOrDefaultAsync();
        }

        public IAsyncEnumerable<ResourceGroup> GetResourceGroupsByTypeAsync(string caseId, ResourceGroupType type)
        {
            return ResourceGroups.GetRecordsAsync<ResourceGroup>(Logger, r => r.CaseId == caseId && r.Type == type.ToString());
        }

        public IAsyncEnumerable<ResourceGroup> GetResourceGroupsByNameAsync(string caseId, string name)
        {
            return ResourceGroups.GetRecordsAsync<ResourceGroup>(Logger, r => r.CaseId == caseId && r.Name.ToLower() == name.ToLower());
        }

        public IAsyncEnumerable<ResourceGroup> GetResourceGroupsAsync(string caseId)
        {
            return ResourceGroups.GetRecordsAsync<ResourceGroup>(Logger, r => r.CaseId == caseId);
        }
        public IAsyncEnumerable<ResourceGroup> GetResourceGroupsAsync<TKey>(Expression<Func<ResourceGroup, bool>> predicate)
        {
            return ResourceGroups.GetRecordsAsync(Logger, predicate);
        }
        public IAsyncEnumerable<ResourceGroup> GetResourceGroupsAsync<TKey>(Expression<Func<ResourceGroup, bool>> predicate, Expression<Func<ResourceGroup, TKey>> keySelector = null, bool descending = false, int skip = 0, int take = -1)
        {
            return ResourceGroups.GetRecordsAsync(Logger, predicate, null, keySelector, descending, skip, take);
        }

        //
        //  Create
        //
        public async Task<ResourceGroup> CreateResourceGroupAsync(string userId, string caseId, ResourceGroupType type, string name, IList<Resource> resources = null)
        {
            var resourceGroup = new ResourceGroup(userId, caseId, type, name);
            resourceGroup.AddResources(resources);
            return await CreateResourceGroupAsync(resourceGroup);
        }
        public async Task<ResourceGroup> CreateResourceGroupAsync(ResourceGroup resourceGroup)
        {
            var response = await ResourceGroups.CreateItemAsync(resourceGroup);
            Logger.LogInformation("CreateResourceGroupAsync RUs {RUs}", response.RequestCharge);
            return response;
        }

        //
        //  Update
        //
        public async Task<ResourceGroup> UpdateResourceGroupAsync(string caseId, string groupId, IList<Resource> resources = null, bool replace = false)
        {
            var resourceGroup = await GetResourceGroupByIdAsync(caseId, groupId);
            if (resourceGroup != null)
            {
                if (replace)
                {
                    resourceGroup.Resources = resources.ToDictionary(r => r.Id);
                }
                else
                {
                    foreach (var resource in resources)
                    {
                        resource.UpdatedOn = DateTimeOffset.UtcNow;
                        resourceGroup.Resources[resource.Id] = resource;
                    }
                }
                return await UpsertResourceGroupAsync(resourceGroup);
            }
            return null;
        }

        public async Task<ResourceGroup> UpsertResourceGroupAsync(string userId, string caseId, ResourceGroupType type, string name, IList<Resource> resources = null, bool replace = false)
        {
            var resourceGroup = await GetResourceGroupByTypeNameAsync(caseId, type, name);
            if (resourceGroup == null)
            {
                resourceGroup = new ResourceGroup(userId, caseId, type, name, resources);
            }
            if (replace)
            {
                resourceGroup.Resources = resources.ToDictionary(r => r.Id);
            }
            else
            {
                foreach (var resource in resources)
                {
                    resource.UpdatedOn = DateTimeOffset.UtcNow;
                    resourceGroup.Resources[resource.Id] = resource;
                }
            }
            return await UpsertResourceGroupAsync(resourceGroup);
        }

        public async Task<ResourceGroup> UpsertResourceGroupAsync(ResourceGroup resourceGroup)
        {
            resourceGroup.UpdatedOn = DateTimeOffset.UtcNow;
            var response = await ResourceGroups.UpsertItemAsync(resourceGroup);
            Logger.LogInformation("UpdateResourceGroupAsync RUs {RUs}", response.RequestCharge);
            return response;
        }

        //
        //  Delete
        //
        public async Task<ResourceGroup> DeleteResourceGroupAsync(string caseId, string groupId)
        {
            var resourceGroup = await GetResourceGroupByIdAsync(caseId, groupId);
            if (resourceGroup != null)
            {
                var response = await ResourceGroups.DeleteItemAsync<ResourceGroup>(groupId, new PartitionKey(caseId));
                Logger.LogInformation("DeleteResourceGroupAsync RUs {RUs}", response.RequestCharge);
                resourceGroup = response;
            }
            return resourceGroup;
        }

        public async Task DeleteResourceGroupsAsync(string userId, string caseId)
        {
            var groups = await GetResourceGroupsAsync(caseId).ToListAsync();
            foreach (var group in groups)
            {
                var response = await ResourceGroups.DeleteItemAsync<ResourceGroup>(group.Id, new PartitionKey(caseId));
                Logger.LogInformation("DeleteCaseResourceGroupAsync RUs {RUs}", response.RequestCharge);
            }
        }
    }
}
