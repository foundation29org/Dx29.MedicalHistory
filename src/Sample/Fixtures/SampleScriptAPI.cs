using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Dx29;
using Dx29.Data;
using Dx29.Web.Services;

namespace Sample
{
    class SampleScriptAPI
    {
        static public async Task RunAsync(MedicalHistoryClient cli, string userId)
        {
            // Create Case
            var mcase = await cli.CreateMedicalCaseAsync(userId, new PatientInfo { Name = "Sample" });
            Console.WriteLine(mcase.Serialize());

            // Update Case
            mcase = await cli.UpdateMedicalCaseAsync(userId, mcase.Id, new PatientInfo { Name = "Sample API", Gender = "male" });
            Console.WriteLine(mcase.Serialize());

            // Create 'Manual' ResourceGroup
            var resGroup = await cli.CreateResourceGroupAsync(userId, mcase.Id, ResourceGroupType.Phenotype, "Manual");
            Console.WriteLine(resGroup.Serialize());

            // Add manual symptoms
            var symptoms = CreateResources(("1", "HP:1", "selected"), ("2", "HP:2", "selected"), ("3", "HP:3", "selected"), ("4", "HP:4", "selected"));
            resGroup = await cli.UpsertResourceGroupAsync(userId, mcase.Id, ResourceGroupType.Phenotype, "Manual", symptoms.ToArray());
            Console.WriteLine(resGroup.Serialize());

            // Uselect symptom 2, 3
            symptoms = CreateResources(("2", "HP:2", "unselected"), ("3", "HP:3", "unselected"));
            resGroup = await cli.UpsertResourceGroupAsync(userId, mcase.Id, ResourceGroupType.Phenotype, "Manual", symptoms.ToArray());
            Console.WriteLine(resGroup.Serialize());

            // Remove symptom 2, 4
            symptoms = CreateResources(("2", "HP:2", "unselected"), ("3", "HP:3", "unselected"));
            await cli.DeleteResourcesAsync(userId, mcase.Id, resGroup.Id, "2");
            await cli.DeleteResourcesAsync(userId, mcase.Id, resGroup.Id, "2", "4");

            // Get Case
            Console.WriteLine("Get Case");
            mcase = await cli.GetMedicalCaseAsync(userId, mcase.Id);
            Console.WriteLine(mcase.Serialize());

            // Get ResourceGroup
            Console.WriteLine("Get ResourceGroup");
            resGroup = await cli.GetResourceGroupByIdAsync(userId, mcase.Id, resGroup.Id);
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
