using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Dx29;
using Dx29.Data;
using Dx29.Services;

namespace Sample
{
    static partial class UseCasesSharing
    {
        static public async Task RunAsync(MedicalHistoryService svc, string userId, string sharedUserId, string sharedEmail)
        {
            // Create main case
            var sharingCase = await CreateMedicalCaseAsync(svc, userId);
            await AddManualSymptomsAsync(svc, sharingCase);

            // Share case
            var medicalCase = await ShareMedicalCaseAsync(svc, sharingCase.UserId, sharingCase.Id, sharedEmail);
            sharingCase = await GetMedicalCaseAsync(svc, sharingCase.UserId, sharingCase.Id);

            // Add symptoms in shared case
            await AddManualSymptomsAsync(svc, medicalCase, 20);

            // Get ResourceGroups
            var resourceGroups = await svc.GetResourceGroupsAsync(medicalCase.UserId, medicalCase.Id);
            Console.WriteLine(resourceGroups.Serialize());

            // Get SharedBy
            var sharedBy = await svc.GetSharedByAsync(medicalCase.UserId, medicalCase.Id);
            Console.WriteLine(sharedBy.Serialize());

            //Console.WriteLine(sharedCase.Serialize());
            //Console.WriteLine(medicalCase.Serialize());

            await svc.StopSharingMedicalCaseAsync(sharingCase.UserId, sharingCase.Id, sharedEmail);

            // Get SharedBy
            sharedBy = await svc.GetSharedByAsync(medicalCase.UserId, medicalCase.Id);
            Console.WriteLine(sharedBy.Serialize());
        }

        private static Resource CreateResource((string, string, string) item)
        {
            return new Resource(item.Item1, item.Item2) { Status = item.Item3 };
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
