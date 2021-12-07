using System;
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
        public async Task<IDictionary<string, IList<Resource>>> GetResourcesAsync(string userId, string caseId, ResourceGroupType? groupType = null, string groupName = null, string resourceId = null)
        {
            return await GetResourcesAsync<Resource>(userId, caseId, groupType, groupName, resourceId);
        }
        public async Task<IDictionary<string, IList<TResource>>> GetResourcesAsync<TResource>(string userId, string caseId, ResourceGroupType? groupType = null, string groupName = null, string resourceId = null) where TResource : Resource
        {
            var args = new List<string>();
            if (groupType != null) args.Add($"groupType={groupType}");
            if (groupName != null) args.Add($"groupName={groupName}");
            if (resourceId != null) args.Add($"resourceId={groupType}");
            if (args.Count > 0)
            {
                string query = String.Join('&', args);
                return await HttpClient.GETAsync<IDictionary<string, IList<TResource>>>($"Resources/{userId}/{caseId}?{query}");
            }
            return await HttpClient.GETAsync<IDictionary<string, IList<TResource>>>($"Resources/{userId}/{caseId}");
        }

        public async Task<IDictionary<string, IList<Resource>>> GetResourcesByTypeAsync(string userId, string caseId, ResourceGroupType type)
        {
            return await GetResourcesByTypeAsync<Resource>(userId, caseId, type);
        }
        public async Task<IDictionary<string, IList<TResource>>> GetResourcesByTypeAsync<TResource>(string userId, string caseId, ResourceGroupType type) where TResource : Resource
        {
            return await HttpClient.GETAsync<IDictionary<string, IList<TResource>>>($"Resources/{userId}/{caseId}?groupType={type}");
        }

        public async Task<IDictionary<string, IList<Resource>>> GetResourcesByNameAsync(string userId, string caseId, string name)
        {
            return await GetResourcesByNameAsync<Resource>(userId, caseId, name);
        }
        public async Task<IDictionary<string, IList<TResource>>> GetResourcesByNameAsync<TResource>(string userId, string caseId, string name) where TResource : Resource
        {
            return await HttpClient.GETAsync<IDictionary<string, IList<TResource>>>($"Resources/{userId}/{caseId}?groupName={name}");
        }

        public async Task<IDictionary<string, IList<Resource>>> GetResourcesByTypeNameAsync(string userId, string caseId, ResourceGroupType type, string name)
        {
            return await GetResourcesByTypeNameAsync<Resource>(userId, caseId, type, name);
        }
        public async Task<IDictionary<string, IList<TResource>>> GetResourcesByTypeNameAsync<TResource>(string userId, string caseId, ResourceGroupType type, string name) where TResource : Resource
        {
            return await HttpClient.GETAsync<IDictionary<string, IList<TResource>>>($"Resources/{userId}/{caseId}?groupType={type}&groupName={name}");
        }

        public async Task<Resource> GetResourceByTypeNameIdAsync(string userId, string caseId, ResourceGroupType type, string name, string resourceId)
        {
            return await GetResourceByTypeNameIdAsync<Resource>(userId, caseId, type, name, resourceId);
        }
        public async Task<TResource> GetResourceByTypeNameIdAsync<TResource>(string userId, string caseId, ResourceGroupType type, string name, string resourceId) where TResource : Resource
        {
            return await HttpClient.GETAsync<TResource>($"Resources/{userId}/{caseId}/{type}/{name}/{resourceId}");
        }

        public async Task<Resource> GetResourceByIdAsync(string userId, string caseId, string groupId, string resourceId)
        {
            return await GetResourceByIdAsync<Resource>(userId, caseId, groupId, resourceId);
        }
        public async Task<TResource> GetResourceByIdAsync<TResource>(string userId, string caseId, string groupId, string resourceId) where TResource : Resource
        {
            return await HttpClient.GETAsync<TResource>($"Resources/{userId}/{caseId}/{groupId}/{resourceId}");
        }

        //
        //  Update
        //
        public async Task<IDictionary<string, IList<Resource>>> UpsertResourcesAsync(string userId, string caseId, IDictionary<string, IList<Resource>> grouped)
        {
            return await HttpClient.PUTAsync<IDictionary<string, IList<Resource>>>($"Resources/{userId}/{caseId}", grouped);
        }

        public async Task<ResourceGroup> UpsertResourcesAsync(string userId, string caseId, ResourceGroupType groupType, string groupName, params Resource[] resources)
        {
            return await HttpClient.PUTAsync<ResourceGroup>($"Resources/{userId}/{caseId}/{groupType}/{groupName}", resources);
        }

        public async Task<ResourceGroup> UpsertResourcesAsync(string userId, string caseId, string resourceGroupId, params Resource[] resources)
        {
            return await HttpClient.PUTAsync<ResourceGroup>($"Resources/{userId}/{caseId}/{resourceGroupId}", resources);
        }

        //
        //  Delete
        //
        public async Task<ResourceGroup> DeleteResourcesAsync(string userId, string caseId, string groupId, params string[] resourceIds)
        {
            if (resourceIds.Length > 0)
            {
                string query = "?resourceId=" + String.Join("&resourceId=", resourceIds);
                return await HttpClient.DELETEAsync<ResourceGroup>($"Resources/{userId}/{caseId}/{groupId}{query}");
            }
            return new ResourceGroup();
        }

        public async Task<ResourceGroup> DeleteResourcesAsync(string userId, string caseId, ResourceGroupType groupType, string groupName, params string[] resourceIds)
        {
            if (resourceIds.Length > 0)
            {
                string query = "?resourceId=" + String.Join("&resourceId=", resourceIds);
                return await HttpClient.DELETEAsync<ResourceGroup>($"Resources/{userId}/{caseId}/{groupType}/{groupName}{query}");
            }
            return new ResourceGroup();
        }
    }
}
