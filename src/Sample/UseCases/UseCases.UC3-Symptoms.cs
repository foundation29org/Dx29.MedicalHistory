using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Dx29.Data;
using Dx29.Services;

namespace Sample
{
    partial class UseCases
    {
        private static async Task<ResourceGroup> AddManualSymptomsAsync(MedicalHistoryService svc, MedicalCase medicalCase)
        {
            var symptoms = CreateResources(
                ("1", "HP:1", "selected"),
                ("2", "HP:2", "selected"),
                ("3", "HP:3", "selected")
            ).ToArray();

            await Task.Delay(1000);
            await svc.UpsertResourceGroupAsync(medicalCase.UserId, medicalCase.Id, ResourceGroupType.Phenotype, "Manual", symptoms);

            symptoms = CreateResources(
                ("3", "HP:3", "selected"),
                ("4", "HP:4", "selected"),
                ("5", "HP:5", "selected")
            ).ToArray();

            await Task.Delay(1000);
            var resourceGroup = await svc.UpsertResourceGroupAsync(medicalCase.UserId, medicalCase.Id, ResourceGroupType.Phenotype, "Manual", symptoms);

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
