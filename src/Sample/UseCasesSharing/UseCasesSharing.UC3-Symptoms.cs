using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Dx29.Data;
using Dx29.Services;

namespace Sample
{
    partial class UseCasesSharing
    {
        private static async Task<ResourceGroup> AddManualSymptomsAsync(MedicalHistoryService svc, MedicalCase medicalCase, int x = 1)
        {
            var symptoms = CreateResources(
                ((x + 0).ToString(), $"HP:{x + 0}", "selected"),
                ((x + 1).ToString(), $"HP:{x + 1}", "selected"),
                ((x + 2).ToString(), $"HP:{x + 2}", "selected")
            ).ToArray();

            await Task.Delay(1000);
            var resourceGroup = await svc.CreateResourceGroupAsync(medicalCase.UserId, medicalCase.Id, ResourceGroupType.Phenotype, $"Manual-{x}", symptoms);

            symptoms = CreateResources(
                ((x + 2).ToString(), $"HP:{x + 2}", "selected"),
                ((x + 3).ToString(), $"HP:{x + 3}", "selected"),
                ((x + 4).ToString(), $"HP:{x + 4}", "selected")
            ).ToArray();

            await Task.Delay(1000);
            resourceGroup = await svc.UpsertResourceGroupAsync(medicalCase.UserId, medicalCase.Id, ResourceGroupType.Phenotype, $"Manual-{x}", symptoms);

            return resourceGroup;
        }

        static private async Task UpdateSymptomsAsync(MedicalHistoryService svc, ResourceGroup resourceGroup)
        {
            var resource1 = resourceGroup.Resources["1"];
            var resource2 = resourceGroup.Resources["2"];

            resource1.Status = "undefined";
            resource2.Status = "unselected";

            await Task.Delay(1000);
            await svc.UpsertResourcesAsync(resourceGroup.UserId, resourceGroup.CaseId, resourceGroup.Id, resource1, resource2);
        }

        static private async Task DeleteSymptomsAsync(MedicalHistoryService svc, ResourceGroup resourceGroup)
        {
            await Task.Delay(1000);
            await svc.DeleteResourcesAsync(resourceGroup.UserId, resourceGroup.CaseId, resourceGroup.Id, "5", "X");
        }

        static private async Task<IDictionary<string, IList<Resource>>> GetAllSymptomsAsync(MedicalHistoryService svc, MedicalCase medicalCase)
        {
            return await svc.GetResourcesByTypeAsync(medicalCase.UserId, medicalCase.Id, ResourceGroupType.Phenotype);
        }

        static private async Task<IList<ResourceGroup>> GetAllSymptomGroupssAsync(MedicalHistoryService svc, MedicalCase medicalCase)
        {
            return await svc.GetResourceGroupsByTypeAsync(medicalCase.UserId, medicalCase.Id, ResourceGroupType.Phenotype);
        }

        static private async Task<ResourceGroup> GetAllMedicalReportsAsync(MedicalHistoryService svc, MedicalCase medicalCase)
        {
            return await svc.GetResourceGroupByTypeNameAsync(medicalCase.UserId, medicalCase.Id, ResourceGroupType.Reports, "Medical");
        }
    }
}
