using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Dx29;
using Dx29.Data;
using Dx29.Services;

namespace Sample
{
    class SampleScript
    {
        static public async Task RunAsync(MedicalHistoryService svc, string userId)
        {
            // Cleanup
            await svc.DeleteUserCasesAsync(userId);

            // Create Case
            var mcase = await svc.CreateMedicalCaseAsync(userId, new PatientInfo { Name = "Sample" });
            Console.WriteLine(mcase.Serialize());

            // Update Case
            mcase = await svc.UpdateMedicalCaseAsync(userId, mcase.Id, new PatientInfo { Name = "Sample Script", Gender = "male" });
            Console.WriteLine(mcase.Serialize());

            // Create 'Manual' ResourceGroup
            var resGroup = await svc.CreateResourceGroupAsync(userId, mcase.Id, ResourceGroupType.Phenotype, "Manual");
            Console.WriteLine(resGroup.Serialize());

            // Add manual symptoms
            var symptoms = CreateResources(("1", "HP:1", "selected"), ("2", "HP:2", "selected"), ("3", "HP:3", "selected"), ("4", "HP:4", "selected"));
            resGroup = await svc.UpsertResourceGroupAsync(userId, mcase.Id, ResourceGroupType.Phenotype, "Manual", symptoms.ToArray());
            Console.WriteLine(resGroup.Serialize());

            // Uselect symptom 2, 3
            symptoms = CreateResources(("2", "HP:2", "unselected"), ("3", "HP:3", "unselected"));
            resGroup = await svc.UpsertResourceGroupAsync(userId, mcase.Id, ResourceGroupType.Phenotype, "Manual", symptoms.ToArray());
            Console.WriteLine(resGroup.Serialize());

            // Remove symptom 2, 4
            symptoms = CreateResources(("2", "HP:2", "unselected"), ("3", "HP:3", "unselected"));
            resGroup = await svc.DeleteResourcesAsync(userId, mcase.Id, resGroup.Id, new string[] { "2" });
            Console.WriteLine(resGroup.Serialize());
            resGroup = await svc.DeleteResourcesAsync(userId, mcase.Id, resGroup.Id, new string[] { "2", "4" });
            Console.WriteLine(resGroup.Serialize());
        }

        private static IEnumerable<Resource> CreateResources(params (string, string, string)[] items)
        {
            foreach (var item in items)
            {
                yield return new Resource(item.Item1, item.Item2) { Status = item.Item3 };
            }
        }
    }
}
