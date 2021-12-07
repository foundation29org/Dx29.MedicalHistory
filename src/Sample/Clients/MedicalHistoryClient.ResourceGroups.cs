using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Dx29.Data;

namespace Dx29.Web.Services
{
    partial class MedicalHistoryClient
    {
        //
        //  Get
        //
        public async Task<IList<ResourceGroup>> GetResourceGroupsAsync(string userId, string caseId)
        {
            return await HttpClient.GETAsync<IList<ResourceGroup>>($"ResourceGroups/{userId}/{caseId}");
        }

        public async Task<ResourceGroup> GetResourceGroupByIdAsync(string userId, string caseId, string groupId)
        {
            return await HttpClient.GETAsync<ResourceGroup>($"ResourceGroups/{userId}/{caseId}/{groupId}");
        }
        public async Task<ResourceGroup<TResource>> GetResourceGroupByIdAsync<TResource>(string userId, string caseId, string groupId) where TResource : Resource
        {
            return await HttpClient.GETAsync<ResourceGroup<TResource>>($"ResourceGroups/{userId}/{caseId}/{groupId}");
        }

        public async Task<IList<ResourceGroup>> GetResourceGroupsByTypeAsync(string userId, string caseId, ResourceGroupType type)
        {
            return await HttpClient.GETAsync<IList<ResourceGroup>>($"ResourceGroups/{userId}/{caseId}?type={type}");
        }
        public async Task<IList<ResourceGroup<TResource>>> GetResourceGroupsByTypeAsync<TResource>(string userId, string caseId, ResourceGroupType type) where TResource : Resource
        {
            return await HttpClient.GETAsync<IList<ResourceGroup<TResource>>>($"ResourceGroups/{userId}/{caseId}?type={type}");
        }

        public async Task<IList<ResourceGroup>> GetResourceGroupsByNameAsync(string userId, string caseId, string name)
        {
            return await HttpClient.GETAsync<IList<ResourceGroup>>($"ResourceGroups/{userId}/{caseId}?name={name}");
        }
        public async Task<IList<ResourceGroup<TResource>>> GetResourceGroupsByNameAsync<TResource>(string userId, string caseId, string name) where TResource : Resource
        {
            return await HttpClient.GETAsync<IList<ResourceGroup<TResource>>>($"ResourceGroups/{userId}/{caseId}?name={name}");
        }

        public async Task<ResourceGroup> GetResourceGroupByTypeNameAsync(string userId, string caseId, ResourceGroupType type, string name)
        {
            return await HttpClient.GETAsync<ResourceGroup>($"ResourceGroups/{userId}/{caseId}?type={type}&name={name}");
        }
        public async Task<ResourceGroup<TResource>> GetResourceGroupByTypeNameAsync<TResource>(string userId, string caseId, ResourceGroupType type, string name) where TResource : Resource
        {
            return await HttpClient.GETAsync<ResourceGroup<TResource>>($"ResourceGroups/{userId}/{caseId}?type={type}&name={name}");
        }

        //
        //  Create
        //
        public async Task<ResourceGroup> CreateResourceGroupAsync(string userId, string caseId, ResourceGroupType type, string name, IList<Resource> resources = null)
        {
            resources = resources ?? new Resource[] { };
            return await HttpClient.POSTAsync<ResourceGroup>($"ResourceGroups/{userId}/{caseId}?type={type}&name={name}", resources);
        }

        //
        //  Update
        //
        public async Task<ResourceGroup> UpdateResourceGroupAsync(ResourceGroup resourceGroup, bool replace = false)
        {
            return await UpsertResourceGroupAsync(resourceGroup.UserId, resourceGroup.CaseId, Enum.Parse<ResourceGroupType>(resourceGroup.Type), resourceGroup.Name, resourceGroup.Resources.Values.ToList(), replace);
        }
        public async Task<ResourceGroup> UpsertResourceGroupAsync(string userId, string caseId, ResourceGroupType type, string name, Resource resource = null, bool replace = false)
        {
            return await UpsertResourceGroupAsync(userId, caseId, type, name, new Resource[] { resource }, replace);
        }
        public async Task<ResourceGroup> UpsertResourceGroupAsync(string userId, string caseId, ResourceGroupType type, string name, IList<Resource> resources = null, bool replace = false)
        {
            return await HttpClient.PUTAsync<ResourceGroup>($"ResourceGroups/{userId}/{caseId}?type={type}&name={name}&replace={replace}", resources);
        }

        //
        //  Delete
        //
        public async Task DeleteResourceGroupAsync(string userId, string caseId, string groupId)
        {
            await HttpClient.DELETEAsync($"ResourceGroups/{userId}/{caseId}/{groupId}");
        }
    }
}
