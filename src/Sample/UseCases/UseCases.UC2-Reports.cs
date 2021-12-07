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
        private static async Task<IList<ResourceGroup>> AddReportAsync(MedicalHistoryService svc, MedicalCase medicalCase)
        {
            var report1 = CreateResource(("r1", "Sample1.txt", "created"));
            var report2 = CreateResource(("r2", "Sample2.txt", "created"));

            await Task.Delay(1000);
            var resourceGroup1 = await svc.UpsertResourceGroupAsync(medicalCase.UserId, medicalCase.Id, ResourceGroupType.Reports, "Medical", report1);
            var resourceGroup2 = await svc.UpsertResourceGroupAsync(medicalCase.UserId, medicalCase.Id, ResourceGroupType.Reports, "Medical", report2);

            return new ResourceGroup[] { resourceGroup1, resourceGroup2 };
        }

        private static async Task ProcessReportsAsync(MedicalHistoryService svc, MedicalCase medicalCase)
        {
            await ProcessReportAsync(svc, medicalCase, "r1", CreateResources(("3", "HP:3", "undefined"), ("4", "HP:4", "unselected"), ("6", "HP:5", "selected"), ("5", "HP:6", "selected")).ToArray());
            await ProcessReportAsync(svc, medicalCase, "r2", CreateResources(("5", "HP:5", "undefined"), ("6", "HP:6", "unselected"), ("7", "HP:7", "selected"), ("8", "HP:8", "selected")).ToArray());
        }
        private static async Task ProcessReportAsync(MedicalHistoryService svc, MedicalCase medicalCase, string reportId, Resource[] resources)
        {
            Console.WriteLine("Processing... {0}", reportId);
            var resource = await svc.GetResourceByTypeNameIdAsync(medicalCase.UserId, medicalCase.Id, ResourceGroupType.Reports, "Medical", reportId);
            if (resource != null)
            {
                await Task.Delay(1000);
                await svc.UpsertResourceGroupAsync(medicalCase.UserId, medicalCase.Id, ResourceGroupType.Phenotype, reportId, resources);

                await Task.Delay(1000);
                resource.Status = "Ready";
                await svc.UpsertResourcesAsync(medicalCase.UserId, medicalCase.Id, ResourceGroupType.Reports, "Medical", resource);
            }
        }
    }
}
