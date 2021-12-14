using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Dx29.Data;

namespace Dx29.Services
{
    // TODO: Check SharedWith not implemented

    partial class MedicalHistoryService
    {
        //
        //  Get
        //
        private async Task<IList<ResourceGroup>> GetSharedResourceGroupsAsync(string userId, string caseId)
        {
            var sharedBy = await GetSharedByInfoAsync(userId, caseId);
            if (sharedBy != null)
            {
                var resourceGroups = await GetResourceGroupsAsync(sharedBy.UserId, sharedBy.CaseId);
                foreach (var resourceGroup in resourceGroups)
                {
                    resourceGroup.UserId = userId;
                    resourceGroup.CaseId = caseId;
                }
                return resourceGroups;
            }
            return null;
        }

        private async Task<ResourceGroup> GetSharedResourceGroupByIdAsync(string userId, string caseId, string groupId)
        {
            var sharedBy = await GetSharedByInfoAsync(userId, caseId);
            if (sharedBy != null)
            {
                var resourceGroup = await GetResourceGroupByIdAsync(sharedBy.UserId, sharedBy.CaseId, groupId);
                resourceGroup.UserId = userId;
                resourceGroup.CaseId = caseId;
                return resourceGroup;
            }
            return null;
        }

        private async Task<ResourceGroup> GetSharedResourceGroupByTypeNameAsync(string userId, string caseId, ResourceGroupType type, string name)
        {
            var sharedBy = await GetSharedByInfoAsync(userId, caseId);
            if (sharedBy != null)
            {
                var resourceGroup = await GetResourceGroupByTypeNameAsync(sharedBy.UserId, sharedBy.CaseId, type, name);
                resourceGroup.UserId = userId;
                resourceGroup.CaseId = caseId;
                return resourceGroup;
            }
            return null;
        }

        private async Task<IList<ResourceGroup>> GetSharedResourceGroupsByTypeAsync(string userId, string caseId, ResourceGroupType type)
        {
            var sharedBy = await GetSharedByInfoAsync(userId, caseId);
            if (sharedBy != null)
            {
                var resourceGroups = await GetResourceGroupsByTypeAsync(sharedBy.UserId, sharedBy.CaseId, type);
                foreach (var resourceGroup in resourceGroups)
                {
                    resourceGroup.UserId = userId;
                    resourceGroup.CaseId = caseId;
                }
                return resourceGroups;
            }
            return null;
        }

        private async Task<IList<ResourceGroup>> GetSharedResourceGroupsByNameAsync(string userId, string caseId, string name)
        {
            var sharedBy = await GetSharedByInfoAsync(userId, caseId);
            if (sharedBy != null)
            {
                var resourceGroups = await GetResourceGroupsByNameAsync(sharedBy.UserId, sharedBy.CaseId, name);
                foreach (var resourceGroup in resourceGroups)
                {
                    resourceGroup.UserId = userId;
                    resourceGroup.CaseId = caseId;
                }
                return resourceGroups;
            }
            return null;
        }

        //
        //  Create
        //
        private async Task<ResourceGroup> CreateSharedResourceGroupAsync(string userId, string caseId, ResourceGroupType type, string name, IList<Resource> resources = null)
        {
            var sharedBy = await GetSharedByInfoAsync(userId, caseId);
            if (sharedBy != null)
            {
                var resourceGroup = await CreateResourceGroupAsync(sharedBy.UserId, sharedBy.CaseId, type, name, resources);
                resourceGroup.UserId = userId;
                resourceGroup.CaseId = caseId;
                return resourceGroup;
            }
            return null;
        }

        //
        //  Update
        //
        private async Task<ResourceGroup> UpdateSharedResourceGroupAsync(string userId, string caseId, string groupId, IList<Resource> resources = null, bool replace = false)
        {
            var sharedBy = await GetSharedByInfoAsync(userId, caseId);
            if (sharedBy != null)
            {
                var resourceGroup = await UpdateResourceGroupAsync(sharedBy.UserId, sharedBy.CaseId, groupId, resources, replace);
                resourceGroup.UserId = userId;
                resourceGroup.CaseId = caseId;
                return resourceGroup;
            }
            return null;
        }

        private async Task<ResourceGroup> UpsertSharedResourceGroupAsync(string userId, string caseId, ResourceGroupType type, string name, IList<Resource> resources = null, bool replace = false)
        {
            var sharedBy = await GetSharedByInfoAsync(userId, caseId);
            if (sharedBy != null)
            {
                var resourceGroup = await UpsertResourceGroupAsync(sharedBy.UserId, sharedBy.CaseId, type, name, resources, replace);
                resourceGroup.UserId = userId;
                resourceGroup.CaseId = caseId;
                return resourceGroup;
            }
            return null;
        }

        public async Task<ResourceGroup> UpdateSharedResourceGroupAsync(ResourceGroup resourceGroup)
        {
            var sharedBy = await GetSharedByInfoAsync(resourceGroup.UserId, resourceGroup.CaseId);
            if (sharedBy != null)
            {
                string userId = resourceGroup.UserId;
                string caseId = resourceGroup.CaseId;
                resourceGroup.UserId = sharedBy.UserId;
                resourceGroup.CaseId = sharedBy.CaseId;
                resourceGroup = await UpdateResourceGroupAsync(resourceGroup);
                resourceGroup.UserId = userId;
                resourceGroup.CaseId = caseId;
                return resourceGroup;
            }
            return null;
        }

        //
        //  Delete
        //
        private async Task DeleteSharedResourceGroupAsync(string userId, string caseId, string groupId)
        {
            var sharedBy = await GetSharedByInfoAsync(userId, caseId);
            if (sharedBy != null)
            {
                await DeleteSharedResourceGroupAsync(sharedBy.UserId, sharedBy.CaseId, groupId);
            }
        }

        //
        //  Helpers
        //
        private async Task<SharedBy> GetSharedByInfoAsync(string userId, string caseId)
        {
            var medicalCase = await MedicalCaseService.GetRawMedicalCaseByIdAsync(userId, caseId);
            return GetSharedByInfo(medicalCase);
        }
        private SharedBy GetSharedByInfo(MedicalCase medicalCase)
        {
            var sharedBy = medicalCase.SharedBy.FirstOrDefault();
            if (sharedBy != null)
            {
                if (sharedBy.Status == SharedByStatus.Accepted.ToString())
                {
                    return sharedBy;
                }
            }
            return null;
        }
    }
}
