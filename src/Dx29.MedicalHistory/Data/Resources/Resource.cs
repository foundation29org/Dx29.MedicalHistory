using System;
using System.Collections.Generic;

namespace Dx29.Data
{
    public class Resource
    {
        public Resource()
        {
            CreatedOn = DateTimeOffset.UtcNow;
            UpdatedOn = CreatedOn;
            Properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
        public Resource(string id, string name) : this()
        {
            Id = id;
            Name = name;
        }

        public string Id { get; set; }
        public string Name { get; set; }

        public string Status { get; set; } // undefined, selected, unselected
        public IDictionary<string, string> Properties { get; set; } // path, score, error, errorMessage

        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }
}
