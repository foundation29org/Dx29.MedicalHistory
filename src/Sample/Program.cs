using System;
using System.IO;
using System.Threading.Tasks;

using Dx29;
using Dx29.Data;
using Dx29.Services;
using Dx29.Web.Services;

namespace Sample
{
    class Program
    {
        const string USER_ID = "auto@sample.com";
        //const string SHARED_USER_ID = "shr-1001";

        static async Task Main(string[] args)
        {
            await RunSharingAsync();
        }

        private static async Task RunSharingAsync()
        {
            await ServiceManager.InitializeAsync();
            var svc = ServiceManager.GetService<MedicalHistoryService>();
            await UseCasesSharingEx.RunAsync(svc);
        }

        private static async Task RunAsync()
        {
            await ServiceManager.InitializeAsync();
            var svc = ServiceManager.GetService<MedicalHistoryService>();
            var cli = ServiceManager.GetService<MedicalHistoryClient>();

            // Encode users
            string userId = svc.MedicalCaseService.AccountHashService.GetHash(USER_ID);
            //string sharedUserId = svc.MedicalCaseService.AccountHashService.GetHash(SHARED_USER_ID);

            // Cleanup
            //await svc.DeleteUserCasesAsync(USER_ID);
            //await svc.DeleteUserCasesAsync(userId);
            //await svc.DeleteUserCasesAsync(sharedUserId);

            // Fixtures
            //await SampleScript.RunAsync(svc, USER_ID);
            //await SampleScriptAPI.RunAsync(cli, USER_ID);

            // Use Cases
            //await UseCases.RunAsync(svc, userId);
            //await UseCasesSharing.RunAsync(svc, userId, sharedUserId, SHARED_USER_ID);
            //await UseCasesSharingEx.RunAsync(svc);

            // Log USER_ID
            //var patientInfo_ = new PatientInfo
            //{
            //    Name = "Orphanet",
            //    BirthDate = null,
            //    Gender = null,
            //};
            //patientInfo_.DiseasesIds.Add("Orphanet:33069");
            //var medicalCase = await svc.CreateMedicalCaseAsync(userId, patientInfo_);
            //var medicalCase = (await svc.GetMedicalCasesAsync(userId)).OrderByDescending(r => r.CreatedOn).FirstOrDefault();
            //if (medicalCase != null)
            //{
            //   await LogMedicalCase(svc, medicalCase);
            //}

            // Log SHARE_USER_ID
            /*medicalCase = (await svc.GetMedicalCasesAsync(sharedUserId)).OrderByDescending(r => r.CreatedOn).FirstOrDefault();
            if (medicalCase != null)
            {
                await LogMedicalCase(svc, medicalCase, "medical-case-shared.json");
                await LogResourceGroups(svc, medicalCase, "resource-groups-shared.json");
            }*/
        }

        #region Log
        private static async Task LogMedicalCase(MedicalHistoryService svc, MedicalCase medicalCase, string filename = "medical-case.json")
        {
            medicalCase = await svc.GetMedicalCaseAsync(medicalCase.UserId, medicalCase.Id);
            Console.WriteLine(medicalCase.Serialize());
            File.WriteAllText($"..\\..\\..\\_schema\\{filename}", medicalCase.Serialize());
        }

        private static async Task LogResourceGroups(MedicalHistoryService svc, MedicalCase medicalCase, string filename = "resource-groups.json")
        {
            var resourceGroups = await svc.GetResourceGroupsAsync(medicalCase.UserId, medicalCase.Id);
            Console.WriteLine(resourceGroups.Serialize());
            File.WriteAllText($"..\\..\\..\\_schema\\{filename}", resourceGroups.Serialize());
        }
        #endregion
    }
}
