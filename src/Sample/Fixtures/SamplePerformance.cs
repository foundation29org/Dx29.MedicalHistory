using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Dx29.Data;
using Dx29.Services;
using Dx29.Tools;

namespace Sample
{
    class SamplePerformance
    {
        const int MEDICAL_CASES = 3;
        const int RESOURCE_GROUPS = 5;
        const int RESOURCES = 10;

        static public async Task RunAsync(MedicalHistoryService svc, string userId)
        {
            await DeleteUserCasesAsync(svc, userId);

            MedicalCase medicalCase = null;
            ResourceGroup resourceGroup = null;
            for (int n = 0; n < MEDICAL_CASES; n++)
            {
                medicalCase = await CreateUserCaseAsync(svc, userId);
                for (int i = 0; i < RESOURCE_GROUPS; i++)
                {
                    resourceGroup = await CreateResourceGroupAsync(svc, userId, medicalCase.Id, $"ResourceName {i}");
                }
                Console.WriteLine(n);
            }
            Console.WriteLine("Case Exsists Ok, {0}", await svc.MedicalCaseExistsAsync(userId, medicalCase.Id));
            Console.WriteLine("Case Exsists KO, {0}", await svc.MedicalCaseExistsAsync(userId, medicalCase.Id + "x"));
        }

        static private async Task DeleteUserCasesAsync(MedicalHistoryService svc, string userId)
        {
            await svc.DeleteUserCasesAsync(userId);
        }

        static private async Task<MedicalCase> CreateUserCaseAsync(MedicalHistoryService svc, string userId)
        {
            var info = new PatientInfo
            {
                Name = "Sample",
                BirthDate = DateTimeOffset.UtcNow.AddDays(-10),
                Gender = "Male"
            };
            return await svc.CreateMedicalCaseAsync(userId, info);
        }

        static private async Task<ResourceGroup> CreateResourceGroupAsync(MedicalHistoryService svc, string userId, string caseId, string resourceName)
        {
            var resources = new List<Resource>();
            for (int n = 0; n < RESOURCES; n++)
            {
                resources.Add(new Resource(Guid.NewGuid().ToString(), IDGenerator.GenerateID()));
            }
            return await svc.CreateResourceGroupAsync(userId, caseId, ResourceGroupType.Phenotype, resourceName, resources);
        }
    }
}
