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
        private static async Task<ResourceGroup> AddTimeLinesAsync(MedicalHistoryService svc, MedicalCase medicalCase)
        {
            var timeLine1 = new Resource(Guid.NewGuid().ToString(), "HP:0000001") { Status = "Ready" };
            var timeLine2 = new Resource(Guid.NewGuid().ToString(), "HP:0000002") { Status = "Ready" };
            var timeLine3 = new Resource(Guid.NewGuid().ToString(), "HP:0000003") { Status = "Ready" };

            var timeLines = new Resource[] { timeLine1, timeLine2, timeLine3 };

            await Task.Delay(1000);
            var resourceGroup = await svc.UpsertResourceGroupAsync(medicalCase.UserId, medicalCase.Id, ResourceGroupType.TimeLine, "TimeLine", timeLines);

            timeLine1.Properties.Add("startDate", DateTimeOffset.UtcNow.ToString("yyyy/MM/dd"));
            timeLine1.Properties.Add("endDate", DateTimeOffset.UtcNow.ToString("yyyy/MM/dd"));
            timeLine1.Properties.Add("isCurrent", true.ToString());
            timeLine1.Properties.Add("Notes", "Some text");

            await Task.Delay(1000);
            resourceGroup = await svc.UpsertResourceGroupAsync(medicalCase.UserId, medicalCase.Id, ResourceGroupType.TimeLine, "TimeLine", timeLines);

            timeLines = new Resource[] { timeLine1, timeLine3 };

            await Task.Delay(1000);
            resourceGroup = await svc.UpsertResourceGroupAsync(medicalCase.UserId, medicalCase.Id, ResourceGroupType.TimeLine, "TimeLine", timeLines, replace: true);

            return resourceGroup;
        }

        static private async Task<IDictionary<string, IList<Resource>>> GetAllTimeLinesAsync(MedicalHistoryService svc, MedicalCase medicalCase)
        {
            return await svc.GetResourcesByTypeAsync(medicalCase.UserId, medicalCase.Id, ResourceGroupType.TimeLine);
        }
    }
}
