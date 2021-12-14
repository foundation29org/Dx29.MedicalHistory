using System;

using Microsoft.Extensions.Configuration;

namespace Dx29.Services
{
    public class CaseRecordsDatabase : DatabaseService
    {
        public CaseRecordsDatabase(IConfiguration configuration)
        {
            AppName = configuration["CaseRecords:AppName"];
            DatabaseName = configuration["CaseRecords:DatabaseName"];
            ConnectionString = configuration["CaseRecords:ConnectionString"];
        }

        public override string AppName { get; }

        public override string DatabaseName { get; }

        public override string ConnectionString { get; }
    }
}
