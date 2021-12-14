using System;
using System.Linq;
using System.Collections.Generic;

namespace Dx29.Data
{
    static public class ResourceGroupExtensions
    {
        static public void AddResource(this ResourceGroup resourceGroup, Resource resource, bool overwrite = false)
        {
            if (overwrite)
            {
                resourceGroup.Resources[resource.Id] = resource;
            }
            else
            {
                resourceGroup.Resources.Add(resource.Id, resource);
            }
        }

        static public void AddResources(this ResourceGroup resourceGroup, IList<Resource> resources, bool overwrite = false)
        {
            if (resources != null)
            {
                foreach (var item in resources.OrderBy(r => r.CreatedOn))
                {
                    AddResource(resourceGroup, item, overwrite);
                }
            }
        }
    }
}
