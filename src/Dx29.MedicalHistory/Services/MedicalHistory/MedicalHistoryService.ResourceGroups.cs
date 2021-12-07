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
        public async Task<IList<ResourceGroup>> GetResourceGroupsAsync(string userId, string caseId)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                return await GetSharedResourceGroupsAsync(userId, caseId);
            }

            if (await MedicalCaseService.MedicalCaseExistsAsync(userId, caseId))
            {
                return await ResourceGroupService.GetResourceGroupsAsync(caseId).ToListAsync();
            }
            return null;
        }

        public async Task<ResourceGroup> GetResourceGroupByIdAsync(string userId, string caseId, string groupId)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                return await GetSharedResourceGroupByIdAsync(userId, caseId, groupId);
            }

            if (await MedicalCaseService.MedicalCaseExistsAsync(userId, caseId))
            {
                return await ResourceGroupService.GetResourceGroupByIdAsync(caseId, groupId);
            }
            return null;
        }

        public async Task<ResourceGroup> GetResourceGroupByTypeNameAsync(string userId, string caseId, ResourceGroupType type, string name)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                return await GetSharedResourceGroupByTypeNameAsync(userId, caseId, type, name);
            }

            if (await MedicalCaseService.MedicalCaseExistsAsync(userId, caseId))
            {
                return await ResourceGroupService.GetResourceGroupByTypeNameAsync(caseId, type, name);
            }
            return null;
        }

        public async Task<IList<ResourceGroup>> GetResourceGroupsByTypeAsync(string userId, string caseId, ResourceGroupType type)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                return await GetSharedResourceGroupsByTypeAsync(userId, caseId, type);
            }

            if (await MedicalCaseService.MedicalCaseExistsAsync(userId, caseId))
            {
                return await ResourceGroupService.GetResourceGroupsByTypeAsync(caseId, type).ToListAsync();
            }
            return null;
        }

        public async Task<IList<ResourceGroup>> GetResourceGroupsByNameAsync(string userId, string caseId, string name)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                return await GetSharedResourceGroupsByNameAsync(userId, caseId, name);
            }

            if (await MedicalCaseService.MedicalCaseExistsAsync(userId, caseId))
            {
                return await ResourceGroupService.GetResourceGroupsByNameAsync(caseId, name).ToListAsync();
            }
            return null;
        }

        //
        //  Create
        //
        public async Task<ResourceGroup> CreateResourceGroupAsync(string userId, string caseId, ResourceGroupType type, string name, IList<Resource> resources = null)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                return await CreateSharedResourceGroupAsync(userId, caseId, type, name, resources);
            }

            var medicalCase = await MedicalCaseService.GetMedicalCaseByIdAsync(userId, caseId);
            if (medicalCase != null)
            {
                var resourceGroup = await ResourceGroupService.GetResourceGroupByTypeNameAsync(caseId, type, name);
                if (resourceGroup == null)
                {
                    resourceGroup = await ResourceGroupService.CreateResourceGroupAsync(userId, caseId, type, name, resources);
                    var resourceGroupRef = CreateResourceGroupRef(resourceGroup);
                    medicalCase.ResourceGroups.Add(resourceGroup.Id, resourceGroupRef);
                    await MedicalCaseService.UpdateMedicalCaseAsync(medicalCase);
                    return resourceGroup;
                }
                throw new InvalidOperationException($"ResourceGroup {type}.{name} already exists.");
            }
            return null;
        }

        //
        //  Update
        //
        public async Task<ResourceGroup> UpdateResourceGroupAsync(string userId, string caseId, string groupId, Resource resource, bool replace = false)
        {
            return await UpdateResourceGroupAsync(userId, caseId, groupId, new Resource[] { resource }, replace);
        }
        public async Task<ResourceGroup> UpdateResourceGroupAsync(string userId, string caseId, string groupId, IList<Resource> resources = null, bool replace = false)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                return await UpdateSharedResourceGroupAsync(userId, caseId, groupId, resources, replace);
            }

            var medicalCase = await MedicalCaseService.GetMedicalCaseByIdAsync(userId, caseId);
            if (medicalCase != null)
            {
                var resourceGroup = await ResourceGroupService.UpdateResourceGroupAsync(caseId, groupId, resources, replace);
                var resourceGroupRef = CreateResourceGroupRef(resourceGroup);
                medicalCase.ResourceGroups[resourceGroup.Id] = resourceGroupRef;
                await MedicalCaseService.UpdateMedicalCaseAsync(medicalCase);
                return resourceGroup;
            }
            return null;
        }

        public async Task<ResourceGroup> UpsertResourceGroupAsync(string userId, string caseId, ResourceGroupType type, string name, Resource resource, bool replace = false)
        {
            return await UpsertResourceGroupAsync(userId, caseId, type, name, new Resource[] { resource }, replace);
        }
        public async Task<ResourceGroup> UpsertResourceGroupAsync(string userId, string caseId, ResourceGroupType type, string name, IList<Resource> resources = null, bool replace = false)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                return await UpsertSharedResourceGroupAsync(userId, caseId, type, name, resources, replace);
            }

            var medicalCase = await MedicalCaseService.GetMedicalCaseByIdAsync(userId, caseId);
            if (medicalCase != null)
            {
                var resourceGroup = await ResourceGroupService.UpsertResourceGroupAsync(userId, caseId, type, name, resources, replace);
                var resourceGroupRef = CreateResourceGroupRef(resourceGroup);
                medicalCase.ResourceGroups[resourceGroup.Id] = resourceGroupRef;
                await MedicalCaseService.UpdateMedicalCaseAsync(medicalCase);
                return resourceGroup;
            }
            return null;
        }

        public async Task<ResourceGroup> UpdateResourceGroupAsync(ResourceGroup resourceGroup)
        {
            if (resourceGroup.CaseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                return await UpdateSharedResourceGroupAsync(resourceGroup);
            }

            var medicalCase = await MedicalCaseService.GetMedicalCaseByIdAsync(resourceGroup.UserId, resourceGroup.CaseId);
            if (medicalCase != null)
            {
                resourceGroup = await ResourceGroupService.UpsertResourceGroupAsync(resourceGroup);
                var resourceGroupRef = CreateResourceGroupRef(resourceGroup);
                medicalCase.ResourceGroups[resourceGroup.Id] = resourceGroupRef;
                await MedicalCaseService.UpdateMedicalCaseAsync(medicalCase);
                return resourceGroup;
            }
            return null;
        }

        //
        //  Delete
        //
        public async Task DeleteResourceGroupAsync(string userId, string caseId, string groupId)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                await DeleteSharedResourceGroupAsync(userId, caseId, groupId);
                return;
            }

            var medicalCase = await MedicalCaseService.GetMedicalCaseByIdAsync(userId, caseId);
            if (medicalCase != null)
            {
                medicalCase.ResourceGroups.Remove(groupId);
                await MedicalCaseService.UpdateMedicalCaseAsync(medicalCase);
                await ResourceGroupService.DeleteResourceGroupAsync(caseId, groupId);
            }
        }

        #region CreateResourceGroupRef
        static private ResourceGroupRef CreateResourceGroupRef(ResourceGroup resourceGroup)
        {
            return CreateResourceGroupRef(resourceGroup.Type, resourceGroup.Name, resourceGroup.Resources.Values.ToArray());
        }
        static private ResourceGroupRef CreateResourceGroupRef(string type, string name, IList<Resource> resources)
        {
            var resourceGroupRef = new ResourceGroupRef(type, name);
            foreach (var resource in resources)
            {
                resourceGroupRef.Resources.Add(resource.Id, resource.Status);
            }
            return resourceGroupRef;
        }
        #endregion
    }
}
