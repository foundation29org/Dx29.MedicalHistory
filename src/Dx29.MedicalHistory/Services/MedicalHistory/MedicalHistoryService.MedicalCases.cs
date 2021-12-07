using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Dx29.Data;

namespace Dx29.Services
{
    partial class MedicalHistoryService
    {
        //
        //  Exists
        //
        public async Task<bool> MedicalCaseExistsAsync(string userId, string caseId)
        {
            return await MedicalCaseService.MedicalCaseExistsAsync(userId, caseId);
        }

        //
        //  Get
        //
        public async Task<IList<MedicalCase>> GetMedicalCasesAsync(string userId, bool includeDeleted = false)
        {
            return await MedicalCaseService.GetMedicalCasesAsync(userId, includeDeleted);
        }

        public async Task<MedicalCase> GetMedicalCaseAsync(string userId, string caseId, bool checkStatus = true)
        {
            return await MedicalCaseService.GetMedicalCaseByIdAsync(userId, caseId, checkStatus);
        }

        //
        //  Create
        //
        public async Task<MedicalCase> CreateMedicalCaseAsync(string userId, PatientInfo info)
        {
            return await MedicalCaseService.CreateMedicalCaseAsync(userId, info);
        }

        //
        //  Share
        //
        public async Task<MedicalCase> ShareMedicalCaseAsync(string userId, string caseId, string targetEmail)
        {
            return await MedicalCaseService.ShareMedicalCaseAsync(userId, caseId, targetEmail);
        }

        public async Task<MedicalCase> AcceptSharingMedicalCaseAsync(string userId, string caseId, string targetEmail)
        {
            return await MedicalCaseService.AcceptSharingMedicalCaseAsync(userId, caseId, targetEmail);
        }

        public async Task<MedicalCase> RevokeSharingMedicalCaseAsync(string userId, string caseId, string targetEmail)
        {
            return await MedicalCaseService.RevokeSharingMedicalCaseAsync(userId, caseId, targetEmail);
        }

        public async Task<MedicalCase> DeleteSharingMedicalCaseAsync(string userId, string caseId, string targetEmail)
        {
            return await MedicalCaseService.DeleteSharingMedicalCaseAsync(userId, caseId, targetEmail);
        }

        public async Task<SharedBy> GetSharedByAsync(string userId, string caseId)
        {
            return await MedicalCaseService.GetSharedByAsync(userId, caseId);
        }

        public async Task StopSharingMedicalCaseAsync(string userId, string caseId, string targetEmail)
        {
            await MedicalCaseService.StopSharingMedicalCaseAsync(userId, caseId, targetEmail);
        }

        //
        //  Update
        //
        public async Task<MedicalCase> UpdateMedicalCaseAsync(string userId, string caseId, PatientInfo info)
        {
            return await MedicalCaseService.UpdateMedicalCaseAsync(userId, caseId, info);
        }

        //
        //  Delete
        //
        public async Task<MedicalCase> DeleteMedicalCaseAsync(string userId, string caseId, bool force = false)
        {
            return await MedicalCaseService.DeleteMedicalCaseAsync(userId, caseId, force);
        }

        public async Task DeleteUserCasesAsync(string userId)
        {
            var medicalCases = await GetMedicalCasesAsync(userId, includeDeleted: true);
            foreach (var medicalCase in medicalCases)
            {
                await ResourceGroupService.DeleteResourceGroupsAsync(medicalCase.UserId, medicalCase.Id);
                await DeleteMedicalCaseAsync(medicalCase.UserId, medicalCase.Id, force: true);
            }
        }
    }
}
