using System;
using System.Collections.Generic;

using Dx29.Tools;

namespace Dx29.Data
{
    #region ResourceGroupType
    public enum ResourceGroupType
    {
        Generic,
        Reports,
        Phenotype,
        Genotype,
        Diseases,
        Analysis,
        Notes,
        TimeLine
    }
    #endregion

    public class ResourceGroup<TResource> where TResource : Resource
    {
        public ResourceGroup()
        {
            Id = IDGenerator.GenerateID('g');
            Resources = new Dictionary<string, TResource>(StringComparer.OrdinalIgnoreCase);
        }
        public ResourceGroup(string userId, string caseId, ResourceGroupType type, string name, IList<TResource> resources = null) : this()
        {
            UserId = userId;
            CaseId = caseId;
            Name = name;
            Type = type.ToString();
            if (resources != null)
            {
                foreach (var resource in resources)
                {
                    Resources[resource.Id] = resource;
                }
            }
            CreatedOn = DateTimeOffset.UtcNow;
            UpdatedOn = CreatedOn;
        }

        public string Id { get; set; }

        public string UserId { get; set; }

        public string CaseId { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public IDictionary<string, TResource> Resources { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }

    public class ResourceGroup : ResourceGroup<Resource>
    {
        public ResourceGroup() : base()
        {
        }
        public ResourceGroup(string userId, string caseId, ResourceGroupType type, string name, IList<Resource> resources = null) : base(userId, caseId, type, name, resources)
        {
        }
    }
}
