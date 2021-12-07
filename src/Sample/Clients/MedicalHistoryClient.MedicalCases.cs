using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Dx29.Data;

namespace Dx29.Web.Services
{
    partial class MedicalHistoryClient
    {
        //
        //  Get
        //
        public async Task<IList<MedicalCase>> GetMedicalCasesAsync(string userId, bool includeDeleted = false)
        {
            return await HttpClient.GETAsync<IList<MedicalCase>>($"MedicalCases/{userId}?includeDeleted={includeDeleted}");
        }

        public async Task<MedicalCase> GetMedicalCaseAsync(string userId, string caseId)
        {
            return await HttpClient.GETAsync<MedicalCase>($"MedicalCases/{userId}/{caseId}");
        }

        //
        //  Create
        //
        public async Task<MedicalCase> CreateMedicalCaseAsync(string userId, PatientInfo info)
        {
            return await HttpClient.POSTAsync<MedicalCase>($"MedicalCases/{userId}", info);
        }

        //
        //  Update
        //
        public async Task<MedicalCase> UpdateMedicalCaseAsync(string userId, string caseId, PatientInfo info)
        {
            return await HttpClient.PATCHAsync<MedicalCase>($"MedicalCases/{userId}/{caseId}", info);
        }

        //
        //  Delete
        //
        public async Task DeleteMedicalCaseAsync(string userId, string caseId, bool force = false)
        {
            await HttpClient.DELETEAsync($"MedicalCases/{userId}/{caseId}?force={force}");
        }
    }
}
