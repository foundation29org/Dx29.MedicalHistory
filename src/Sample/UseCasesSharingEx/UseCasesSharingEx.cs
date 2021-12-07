using System;
using System.Threading.Tasks;

using Dx29;
using Dx29.Data;
using Dx29.Services;

namespace Sample
{
    static partial class UseCasesSharingEx
    {
        const string USR_MAIN = "main@sample.com";
        const string USR_SHARE_A = "shareA@sample.com";
        const string USR_SHARE_B = "shareB@sample.com";
        const string USR_SHARE_C = "shareC@sample.com";

        static public async Task RunAsync(MedicalHistoryService svc)
        {
            await CleanupAsync(svc);

            await Case1(svc);
            //await Case2(svc);
            //await Case3(svc);
            //await Case4(svc);
            //await Case41(svc);
            //await Case42(svc);
            //await Case5(svc);
            //await Case6(svc);
        }

        #region Case 1: Owner share to A. A share to B
        private static async Task Case1(MedicalHistoryService svc)
        {
            var mainId = GetUserId(svc, USR_MAIN);
            var mainCase = await svc.CreateMedicalCaseAsync(mainId, new PatientInfo { Name = "Main" });

            var caseA = await svc.ShareMedicalCaseAsync(mainId, mainCase.Id, USR_SHARE_A);

            bool invalid = false;
            try
            {
                caseA = await svc.ShareMedicalCaseAsync(mainId, mainCase.Id, USR_SHARE_A);
                invalid = true;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            if (invalid)
            {
                throw new InvalidOperationException("Expected exception: This medical case is already shared with this user.");
            }
            var caseB = await svc.ShareMedicalCaseAsync(caseA.UserId, caseA.Id, USR_SHARE_B);

            mainCase = await svc.GetMedicalCaseAsync(mainId, mainCase.Id);
            caseA = await svc.GetMedicalCaseAsync(caseA.UserId, caseA.Id);
            caseB = await svc.GetMedicalCaseAsync(caseB.UserId, caseB.Id, checkStatus: false);
            FConsole.WriteLine(mainCase.Serialize());
            FConsole.WriteLine(caseA.Serialize());
            FConsole.WriteLine(caseB.Serialize());
        }
        #endregion

        #region Case 2: Owner share to A. A share to B. B share to C
        private static async Task Case2(MedicalHistoryService svc)
        {
            var mainId = GetUserId(svc, USR_MAIN);
            var mainCase = await svc.CreateMedicalCaseAsync(mainId, new PatientInfo { Name = "Main" });

            var caseA = await svc.ShareMedicalCaseAsync(mainId, mainCase.Id, USR_SHARE_A);
            var caseB = await svc.ShareMedicalCaseAsync(caseA.UserId, caseA.Id, USR_SHARE_B);

            bool invalid = false;
            try
            {
                await svc.ShareMedicalCaseAsync(caseB.UserId, caseB.Id, USR_SHARE_C);
                invalid = true;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            if (invalid)
            {
                throw new InvalidOperationException("Expected exception: Medical case awaiting approval.");
            }

            await svc.AcceptSharingMedicalCaseAsync(mainId, mainCase.Id, USR_SHARE_B);
            var caseC = await svc.ShareMedicalCaseAsync(caseB.UserId, caseB.Id, USR_SHARE_C);

            mainCase = await svc.GetMedicalCaseAsync(mainId, mainCase.Id);
            caseA = await svc.GetMedicalCaseAsync(caseA.UserId, caseA.Id);
            caseB = await svc.GetMedicalCaseAsync(caseB.UserId, caseB.Id, checkStatus: false);
            caseC = await svc.GetMedicalCaseAsync(caseC.UserId, caseC.Id, checkStatus: false);
            FConsole.WriteLine(mainCase.Serialize());
            FConsole.WriteLine(caseA.Serialize());
            FConsole.WriteLine(caseB.Serialize());
            FConsole.WriteLine(caseC.Serialize());
        }
        #endregion

        #region Case 3: Only owner can accept sharing
        private static async Task Case3(MedicalHistoryService svc)
        {
            var mainId = GetUserId(svc, USR_MAIN);
            var mainCase = await svc.CreateMedicalCaseAsync(mainId, new PatientInfo { Name = "Main" });

            var caseA = await svc.ShareMedicalCaseAsync(mainId, mainCase.Id, USR_SHARE_A);
            var caseB = await svc.ShareMedicalCaseAsync(caseA.UserId, caseA.Id, USR_SHARE_B);

            bool invalid = false;
            try
            {
                await svc.AcceptSharingMedicalCaseAsync(caseB.UserId, caseB.Id, USR_SHARE_B);
                invalid = true;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            if (invalid)
            {
                throw new InvalidOperationException("Expected exception: Only owner can accept sharing this case.");
            }

            mainCase = await svc.GetMedicalCaseAsync(mainId, mainCase.Id);
            caseA = await svc.GetMedicalCaseAsync(caseA.UserId, caseA.Id);
            caseB = await svc.GetMedicalCaseAsync(caseB.UserId, caseB.Id, checkStatus: false);
            FConsole.WriteLine(mainCase.Serialize());
            FConsole.WriteLine(caseA.Serialize());
            FConsole.WriteLine(caseB.Serialize());
        }
        #endregion

        #region Case 4: Owner delete sharing A
        private static async Task Case4(MedicalHistoryService svc)
        {
            var mainId = GetUserId(svc, USR_MAIN);
            var mainCase = await svc.CreateMedicalCaseAsync(mainId, new PatientInfo { Name = "Main" });

            var caseA = await svc.ShareMedicalCaseAsync(mainId, mainCase.Id, USR_SHARE_A);
            var caseB = await svc.ShareMedicalCaseAsync(caseA.UserId, caseA.Id, USR_SHARE_B);

            await svc.AcceptSharingMedicalCaseAsync(mainCase.UserId, mainCase.Id, USR_SHARE_B);

            mainCase = await svc.GetMedicalCaseAsync(mainId, mainCase.Id);
            caseA = await svc.GetMedicalCaseAsync(caseA.UserId, caseA.Id);
            caseB = await svc.GetMedicalCaseAsync(caseB.UserId, caseB.Id, checkStatus: false);
            FConsole.WriteLine(mainCase.Serialize());
            FConsole.WriteLine(caseA.Serialize());
            FConsole.WriteLine(caseB.Serialize());

            await svc.DeleteSharingMedicalCaseAsync(mainCase.UserId, mainCase.Id, USR_SHARE_A);

            mainCase = await svc.GetMedicalCaseAsync(mainId, mainCase.Id);
            caseA = await svc.GetMedicalCaseAsync(caseA.UserId, caseA.Id, checkStatus: false);
            caseB = await svc.GetMedicalCaseAsync(caseB.UserId, caseB.Id);
            FConsole.WriteLine(mainCase.Serialize());
            FConsole.WriteLine(caseA.Serialize());
            FConsole.WriteLine(caseB.Serialize());
        }
        #endregion

        #region Case 41: Owner delete sharing B
        private static async Task Case41(MedicalHistoryService svc)
        {
            var mainId = GetUserId(svc, USR_MAIN);
            var mainCase = await svc.CreateMedicalCaseAsync(mainId, new PatientInfo { Name = "Main" });

            var caseA = await svc.ShareMedicalCaseAsync(mainId, mainCase.Id, USR_SHARE_A);
            var caseB = await svc.ShareMedicalCaseAsync(caseA.UserId, caseA.Id, USR_SHARE_B);
            await svc.AcceptSharingMedicalCaseAsync(mainId, mainCase.Id, USR_SHARE_B);
            var caseC = await svc.ShareMedicalCaseAsync(caseB.UserId, caseB.Id, USR_SHARE_C);

            mainCase = await svc.GetMedicalCaseAsync(mainId, mainCase.Id);
            caseA = await svc.GetMedicalCaseAsync(caseA.UserId, caseA.Id);
            caseB = await svc.GetMedicalCaseAsync(caseB.UserId, caseB.Id, checkStatus: false);
            caseC = await svc.GetMedicalCaseAsync(caseC.UserId, caseC.Id, checkStatus: false);
            FConsole.WriteLine(mainCase.Serialize());
            FConsole.WriteLine(caseA.Serialize());
            FConsole.WriteLine(caseB.Serialize());
            FConsole.WriteLine(caseC.Serialize());

            await svc.DeleteSharingMedicalCaseAsync(mainCase.UserId, mainCase.Id, USR_SHARE_B);

            mainCase = await svc.GetMedicalCaseAsync(mainId, mainCase.Id);
            caseA = await svc.GetMedicalCaseAsync(caseA.UserId, caseA.Id);
            caseB = await svc.GetMedicalCaseAsync(caseB.UserId, caseB.Id, checkStatus: false);
            caseC = await svc.GetMedicalCaseAsync(caseC.UserId, caseC.Id, checkStatus: false);
            FConsole.WriteLine(mainCase.Serialize());
            FConsole.WriteLine(caseA.Serialize());
            FConsole.WriteLine(caseB.Serialize());
            FConsole.WriteLine(caseC.Serialize());
        }
        #endregion

        #region Case 42: Owner delete sharing C
        private static async Task Case42(MedicalHistoryService svc)
        {
            var mainId = GetUserId(svc, USR_MAIN);
            var mainCase = await svc.CreateMedicalCaseAsync(mainId, new PatientInfo { Name = "Main" });

            var caseA = await svc.ShareMedicalCaseAsync(mainId, mainCase.Id, USR_SHARE_A);
            var caseB = await svc.ShareMedicalCaseAsync(caseA.UserId, caseA.Id, USR_SHARE_B);
            await svc.AcceptSharingMedicalCaseAsync(mainId, mainCase.Id, USR_SHARE_B);
            var caseC = await svc.ShareMedicalCaseAsync(caseB.UserId, caseB.Id, USR_SHARE_C);

            mainCase = await svc.GetMedicalCaseAsync(mainId, mainCase.Id);
            caseA = await svc.GetMedicalCaseAsync(caseA.UserId, caseA.Id);
            caseB = await svc.GetMedicalCaseAsync(caseB.UserId, caseB.Id, checkStatus: false);
            caseC = await svc.GetMedicalCaseAsync(caseC.UserId, caseC.Id, checkStatus: false);
            FConsole.WriteLine(mainCase.Serialize());
            FConsole.WriteLine(caseA.Serialize());
            FConsole.WriteLine(caseB.Serialize());
            FConsole.WriteLine(caseC.Serialize());

            await svc.DeleteSharingMedicalCaseAsync(mainCase.UserId, mainCase.Id, USR_SHARE_C);

            mainCase = await svc.GetMedicalCaseAsync(mainId, mainCase.Id);
            caseA = await svc.GetMedicalCaseAsync(caseA.UserId, caseA.Id);
            caseB = await svc.GetMedicalCaseAsync(caseB.UserId, caseB.Id, checkStatus: false);
            caseC = await svc.GetMedicalCaseAsync(caseC.UserId, caseC.Id, checkStatus: false);
            FConsole.WriteLine(mainCase.Serialize());
            FConsole.WriteLine(caseA.Serialize());
            FConsole.WriteLine(caseB.Serialize());
            FConsole.WriteLine(caseC.Serialize());
        }
        #endregion

        #region Case 5: A delete sharing B
        private static async Task Case5(MedicalHistoryService svc)
        {
            var mainId = GetUserId(svc, USR_MAIN);
            var mainCase = await svc.CreateMedicalCaseAsync(mainId, new PatientInfo { Name = "Main" });

            var caseA = await svc.ShareMedicalCaseAsync(mainId, mainCase.Id, USR_SHARE_A);
            var caseB = await svc.ShareMedicalCaseAsync(caseA.UserId, caseA.Id, USR_SHARE_B);
            await svc.AcceptSharingMedicalCaseAsync(mainId, mainCase.Id, USR_SHARE_B);
            var caseC = await svc.ShareMedicalCaseAsync(caseB.UserId, caseB.Id, USR_SHARE_C);

            mainCase = await svc.GetMedicalCaseAsync(mainId, mainCase.Id);
            caseA = await svc.GetMedicalCaseAsync(caseA.UserId, caseA.Id);
            caseB = await svc.GetMedicalCaseAsync(caseB.UserId, caseB.Id, checkStatus: false);
            caseC = await svc.GetMedicalCaseAsync(caseC.UserId, caseC.Id, checkStatus: false);
            FConsole.WriteLine(mainCase.Serialize());
            FConsole.WriteLine(caseA.Serialize());
            FConsole.WriteLine(caseB.Serialize());
            FConsole.WriteLine(caseC.Serialize());

            await svc.DeleteSharingMedicalCaseAsync(caseA.UserId, caseA.Id, USR_SHARE_B);

            mainCase = await svc.GetMedicalCaseAsync(mainId, mainCase.Id);
            caseA = await svc.GetMedicalCaseAsync(caseA.UserId, caseA.Id);
            caseB = await svc.GetMedicalCaseAsync(caseB.UserId, caseB.Id, checkStatus: false);
            caseC = await svc.GetMedicalCaseAsync(caseC.UserId, caseC.Id, checkStatus: false);
            FConsole.WriteLine(mainCase.Serialize());
            FConsole.WriteLine(caseA.Serialize());
            FConsole.WriteLine(caseB.Serialize());
            FConsole.WriteLine(caseC.Serialize());
        }
        #endregion

        #region Case 6: B delete sharing
        private static async Task Case6(MedicalHistoryService svc)
        {
            var mainId = GetUserId(svc, USR_MAIN);
            var mainCase = await svc.CreateMedicalCaseAsync(mainId, new PatientInfo { Name = "Main" });

            var caseA = await svc.ShareMedicalCaseAsync(mainId, mainCase.Id, USR_SHARE_A);
            var caseB = await svc.ShareMedicalCaseAsync(caseA.UserId, caseA.Id, USR_SHARE_B);
            await svc.AcceptSharingMedicalCaseAsync(mainId, mainCase.Id, USR_SHARE_B);
            var caseC = await svc.ShareMedicalCaseAsync(caseB.UserId, caseB.Id, USR_SHARE_C);

            mainCase = await svc.GetMedicalCaseAsync(mainId, mainCase.Id);
            caseA = await svc.GetMedicalCaseAsync(caseA.UserId, caseA.Id);
            caseB = await svc.GetMedicalCaseAsync(caseB.UserId, caseB.Id, checkStatus: false);
            caseC = await svc.GetMedicalCaseAsync(caseC.UserId, caseC.Id, checkStatus: false);
            FConsole.WriteLine(mainCase.Serialize());
            FConsole.WriteLine(caseA.Serialize());
            FConsole.WriteLine(caseB.Serialize());
            FConsole.WriteLine(caseC.Serialize());

            await svc.DeleteSharingMedicalCaseAsync(caseB.UserId, caseB.Id, USR_SHARE_C);

            mainCase = await svc.GetMedicalCaseAsync(mainId, mainCase.Id);
            caseA = await svc.GetMedicalCaseAsync(caseA.UserId, caseA.Id);
            caseB = await svc.GetMedicalCaseAsync(caseB.UserId, caseB.Id, checkStatus: false);
            caseC = await svc.GetMedicalCaseAsync(caseC.UserId, caseC.Id, checkStatus: false);
            FConsole.WriteLine(mainCase.Serialize());
            FConsole.WriteLine(caseA.Serialize());
            FConsole.WriteLine(caseB.Serialize());
            FConsole.WriteLine(caseC.Serialize());
        }
        #endregion

        #region Helpers
        private static async Task CleanupAsync(MedicalHistoryService svc)
        {
            await DeleteUserCasesAsync(svc, USR_MAIN);
            await DeleteUserCasesAsync(svc, USR_SHARE_A);
            await DeleteUserCasesAsync(svc, USR_SHARE_B);
            await DeleteUserCasesAsync(svc, USR_SHARE_C);
        }

        private static async Task DeleteUserCasesAsync(MedicalHistoryService svc, string email)
        {
            string userId = GetUserId(svc, email);
            await svc.DeleteUserCasesAsync(userId);
            Console.WriteLine("Deleted cases by {0}", userId);
        }

        private static string GetUserId(MedicalHistoryService svc, string email)
        {
            return svc.MedicalCaseService.AccountHashService.GetHash(email);
        }
        #endregion
    }
}
