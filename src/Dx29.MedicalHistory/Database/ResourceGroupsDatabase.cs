using System;

using Microsoft.Extensions.Configuration;

namespace Dx29.Services
{
    public class ResourceGroupsDatabase : DatabaseService
    {
        public ResourceGroupsDatabase(IConfiguration configuration)
        {
            AppName = configuration["ResourceGroups:AppName"];
            DatabaseName = configuration["ResourceGroups:DatabaseName"];
            ConnectionString = configuration["ResourceGroups:ConnectionString"];
        }

        public override string AppName { get; }

        public override string DatabaseName { get; }

        public override string ConnectionString { get; }
    }
}
