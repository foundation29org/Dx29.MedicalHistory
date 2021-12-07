using System;
using System.Collections.Generic;

namespace Dx29.Data
{
    public class ResourceGroupRef
    {
        public ResourceGroupRef()
        {
            Resources = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
        public ResourceGroupRef(string type, string name) : this()
        {
            Type = type;
            Name = name;
            LastUpdate = DateTimeOffset.UtcNow;
        }

        public string Type { get; set; }
        public string Name { get; set; }
        public IDictionary<string, string> Resources { get; set; }
        public DateTimeOffset LastUpdate { get; set; }
    }
}
