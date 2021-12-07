using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Dx29.Data;

namespace Dx29.Services
{
    partial class MedicalHistoryService
    {
        //
        //  Get
        //
        public async Task<IDictionary<string, IList<Resource>>> GetResourcesAsync(string userId, string caseId, ResourceGroupType? groupType = null, string groupName = null, string resourceId = null)
        {
            IDictionary<string, IList<Resource>> grouped = new Dictionary<string, IList<Resource>>();
            IList<ResourceGroup> resourceGroups = null;

            if (groupType != null && groupName != null)
            {
                var resourceGroup = await GetResourceGroupByTypeNameAsync(userId, caseId, groupType.Value, groupName);
                if (resourceGroup == null) return null;
                resourceGroups = new List<ResourceGroup>() { resourceGroup };
            }
            else if (groupType != null)
            {
                resourceGroups = await GetResourceGroupsByTypeAsync(userId, caseId, groupType.Value);
                if (resourceGroups == null) return null;
            }
            else if (groupName != null)
            {
                resourceGroups = await GetResourceGroupsByNameAsync(userId, caseId, groupName);
                if (resourceGroups == null) return null;
            }
            if (resourceGroups == null)
            {
                resourceGroups = await GetResourceGroupsAsync(userId, caseId);
                if (resourceGroups == null) return null;
            }
            foreach (var resourceGroup in resourceGroups)
            {
                string key = $"{resourceGroup.Id}.{resourceGroup.Name}";
                if (resourceId != null)
                {
                    grouped[key] = resourceGroup.Resources.Where(r => r.Key.EqualsNoCase(resourceId)).Select(r => r.Value).ToList();
                }
                else
                {
                    grouped[key] = resourceGroup.Resources.Values.ToList();
                }
            }
            return grouped;
        }

        public async Task<IDictionary<string, IList<Resource>>> GetResourcesByTypeAsync(string userId, string caseId, ResourceGroupType groupType, string resourceId = null)
        {
            return await GetResourcesAsync(userId, caseId, groupType: groupType, groupName: null, resourceId);
        }

        public async Task<IDictionary<string, IList<Resource>>> GetResourcesByNameAsync(string userId, string caseId, string groupName, string resourceId = null)
        {
            return await GetResourcesAsync(userId, caseId, groupType: null, groupName: groupName, resourceId);
        }

        public async Task<Resource> GetResourceByTypeNameIdAsync(string userId, string caseId, ResourceGroupType groupType, string groupName, string resourceId)
        {
            var resourceGroup = await GetResourceGroupByTypeNameAsync(userId, caseId, groupType, groupName);
            if (resourceGroup != null)
            {
                return resourceGroup.Resources.TryGetValue(resourceId);
            }
            return null;
        }

        public async Task<Resource> GetResourceByIdAsync(string userId, string caseId, string groupId, string resourceId)
        {
            var resourceGroup = await GetResourceGroupByIdAsync(userId, caseId, groupId);
            if (resourceGroup != null)
            {
                return resourceGroup.Resources.TryGetValue(resourceId);
            }
            return null;
        }

        //
        //  Update
        //
        public async Task<IDictionary<string, IList<Resource>>> UpsertResourcesAsync(string userId, string caseId, IDictionary<string, IList<Resource>> grouped)
        {
            var groupedResult = new Dictionary<string, IList<Resource>>();
            foreach (var group in grouped)
            {
                string key = group.Key;
                string groupId = key.Split('.').First();
                var resourceGroup = await UpsertResourcesAsync(userId, caseId, groupId, group.Value.ToArray());
                groupedResult[key] = resourceGroup?.Resources?.Values?.ToList();
            }
            return groupedResult;
        }

        public async Task<ResourceGroup> UpsertResourcesAsync(string userId, string caseId, ResourceGroupType groupType, string groupName, params Resource[] resources)
        {
            var resourceGroup = await GetResourceGroupByTypeNameAsync(userId, caseId, groupType, groupName);
            if (resourceGroup != null)
            {
                foreach (var resource in resources)
                {
                    resource.UpdatedOn = DateTimeOffset.UtcNow;
                    resourceGroup.Resources[resource.Id] = resource;
                }
                resourceGroup = await UpdateResourceGroupAsync(resourceGroup);
            }
            return resourceGroup;
        }

        public async Task<ResourceGroup> UpsertResourcesAsync(string userId, string caseId, string resourceGroupId, params Resource[] resources)
        {
            var resourceGroup = await GetResourceGroupByIdAsync(userId, caseId, resourceGroupId);
            if (resourceGroup != null)
            {
                foreach (var resource in resources)
                {
                    resource.UpdatedOn = DateTimeOffset.UtcNow;
                    resourceGroup.Resources[resource.Id] = resource;
                }
                resourceGroup = await UpdateResourceGroupAsync(resourceGroup);
            }
            return resourceGroup;
        }

        //
        //  Delete
        //
        public async Task<IDictionary<string, IList<Resource>>> DeleteResourcesAsync(string userId, string caseId, IDictionary<string, IList<Resource>> grouped)
        {
            var groupedResult = new Dictionary<string, IList<Resource>>();
            foreach (var group in grouped)
            {
                string key = group.Key;
                string groupId = key.Split('.').First();
                var resourceGroup = await DeleteResourcesAsync(userId, caseId, groupId, group.Value.Select(r => r.Id).ToArray());
                groupedResult[key] = resourceGroup == null ? null : new Resource[] { };
            }
            return groupedResult;
        }

        public async Task<ResourceGroup> DeleteResourcesAsync(string userId, string caseId, string groupId, params string[] resourceIds)
        {
            var resourceGroup = await GetResourceGroupByIdAsync(userId, caseId, groupId);
            if (resourceGroup != null)
            {
                return await DeleteResourcesAsync(resourceGroup, resourceIds);
            }
            return null;
        }

        public async Task<ResourceGroup> DeleteResourcesAsync(string userId, string caseId, ResourceGroupType groupType, string groupName, params string[] resourceIds)
        {
            var resourceGroup = await GetResourceGroupByTypeNameAsync(userId, caseId, groupType, groupName);
            if (resourceGroup != null)
            {
                return await DeleteResourcesAsync(resourceGroup, resourceIds);
            }
            return null;
        }

        private async Task<ResourceGroup> DeleteResourcesAsync(ResourceGroup resourceGroup, IList<string> resourceIds)
        {
            foreach (var resourceId in resourceIds)
            {
                if (resourceGroup.Resources.ContainsKey(resourceId))
                {
                    resourceGroup.Resources.Remove(resourceId);
                }
            }
            resourceGroup = await UpdateResourceGroupAsync(resourceGroup);
            return resourceGroup;
        }
    }
}
