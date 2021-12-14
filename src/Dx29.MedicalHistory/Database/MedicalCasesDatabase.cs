using System;

using Microsoft.Extensions.Configuration;

namespace Dx29.Services
{
    public class MedicalCasesDatabase : DatabaseService
    {
        public MedicalCasesDatabase(IConfiguration configuration)
        {
            AppName = configuration["MedicalCases:AppName"];
            DatabaseName = configuration["MedicalCases:DatabaseName"];
            ConnectionString = configuration["MedicalCases:ConnectionString"];
        }

        public override string AppName { get; }

        public override string DatabaseName { get; }

        public override string ConnectionString { get; }
    }
}
