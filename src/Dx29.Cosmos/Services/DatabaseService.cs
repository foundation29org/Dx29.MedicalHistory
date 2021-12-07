using System;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;

namespace Dx29.Services
{
    abstract public class DatabaseService
    {
        abstract public string AppName { get; }
        abstract public string DatabaseName { get; }
        abstract public string ConnectionString { get; }

        public CosmosClient Client { get; private set; }
        public Database Database { get; private set; }

        public async Task InitializeAsync()
        {
            Client = new CosmosClientBuilder(ConnectionString)
                .WithApplicationName(AppName)
                .WithSerializerOptions(new CosmosSerializationOptions { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase })
                .Build();

            var response = await Client.CreateDatabaseIfNotExistsAsync(DatabaseName);
            Database = response.Database;
        }
    }
}
