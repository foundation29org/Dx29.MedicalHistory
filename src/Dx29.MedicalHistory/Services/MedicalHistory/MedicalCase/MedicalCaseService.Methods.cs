using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

using Dx29.Data;

namespace Dx29.Services
{
    partial class MedicalCaseService
    {
        //
        //  Exists
        //
        public async Task<bool> MedicalCaseExistsAsync(string userId, string caseId)
        {
            return await MedicalCases.RecordExistsAsync(caseId, new PartitionKey(userId));
        }

        //
        //  Get
        //
        public async Task<MedicalCase> GetMedicalCaseByIdAsync(string userId, string caseId, bool checkStatus = true)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                return await GetSharedMedicalCaseByIdAsync(userId, caseId, checkStatus);
            }

            var medicalCase = await GetRawMedicalCaseByIdAsync(userId, caseId);
            if (medicalCase != null)
            {
                var record = await CaseRecordService.GetCaseRecordByIdAsync(userId, caseId);
                medicalCase.PatientInfo = record.AsPatientInfo();
                return medicalCase;
            }
            return medicalCase;
        }

        // Don't use. Use GetMainMedicalCaseAsync instead
        //public async Task<MedicalCase> GetMainMedicalCaseByIdAsync(string userId, string caseId)
        //{
        //    var medicalCase = await GetRawMedicalCaseByIdAsync(userId, caseId);
        //    if (medicalCase != null)
        //    {
        //        if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
        //        {
        //            var sharedBy = medicalCase.SharedBy.First();
        //            return await GetMainMedicalCaseByIdAsync(sharedBy.UserId, sharedBy.CaseId);
        //        }
        //    }
        //    return medicalCase;
        //}

        private async Task<MedicalCase> GetMainMedicalCaseAsync(MedicalCase sharingCase)
        {
            if (sharingCase.Id.StartsWith('c'))
            {
                return sharingCase;
            }
            (var userId, var caseId) = sharingCase.SharedBy.Where(r => r.CaseId.StartsWith('c')).Select(r => (r.UserId, r.CaseId)).FirstOrDefault();
            if (userId != null && caseId != null)
            {
                return await GetRawMedicalCaseByIdAsync(userId, caseId);
            }
            return null;
        }

        public async Task<MedicalCase> GetRawMedicalCaseByIdAsync(string userId, string caseId)
        {
            try
            {
                var response = await MedicalCases.ReadItemAsync<MedicalCase>(caseId, new PartitionKey(userId));
                Logger.LogInformation("GetMedicalCaseByIdAsync RUs {RUs}", response.RequestCharge);
                return response.Resource;
            }
            catch (CosmosException ex)
            {
                if (ex.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    Logger.LogError("GetMedicalCaseByIdAsync. {exception}", ex);
                    throw;
                }
                Logger.LogWarning("GetMedicalCaseByIdAsync. Not found.");
            }
            catch (Exception ex)
            {
                Logger.LogError("GetMedicalCaseByIdAsync. {exception}", ex);
            }
            return null;
        }

        public async Task<IList<MedicalCase>> GetMedicalCasesAsync(string userId, bool includeDeleted = false)
        {
            string filter = includeDeleted ? "none" : CaseStatus.Deleted.ToString();
            var records = (await CaseRecordService.GetCaseRecordsAsync(userId).ToListAsync()).ToDictionary(r => r.Id);
            var items = await MedicalCases.GetRecordsAsync<MedicalCase, DateTimeOffset>(Logger, r => r.UserId == userId, r => r.Status != filter, r => r.CreatedOn, true).ToListAsync();
            items = items.Where(r =>
                    r.Status != CaseStatus.PendingShared.ToString() &&
                    r.Status != CaseStatus.StopShared.ToString()
                )
            .ToList();
            foreach (var item in items)
            {
                if (item.Id.StartsWith("c", StringComparison.OrdinalIgnoreCase))
                {
                    var id = CaseRecordService.RecordHashService.GetHash(item.Id);
                    item.PatientInfo = records.TryGetValue(id).AsPatientInfo();
                }
                else
                {
                    await MergeSharedMedicalCaseAsync(item);
                }
            }
            return items;
        }

        public async Task<IList<MedicalCase>> GetRawMedicalCasesAsync(string userId, bool includeDeleted = false)
        {
            string filter = includeDeleted ? "none" : CaseStatus.Deleted.ToString();
            var records = (await CaseRecordService.GetCaseRecordsAsync(userId).ToListAsync()).ToDictionary(r => r.Id);
            var items = await MedicalCases.GetRecordsAsync<MedicalCase, DateTimeOffset>(Logger, r => r.UserId == userId, r => r.Status != filter, r => r.CreatedOn, true).ToListAsync();
            return items;
        }

        //
        //  Create
        //
        public async Task<MedicalCase> CreateMedicalCaseAsync(string userId, PatientInfo info)
        {

            var medicalCase = new MedicalCase(userId, false, info.DiseasesIds);
            var response = await MedicalCases.CreateItemAsync(medicalCase);
            Logger.LogInformation("CreateMedicalCaseAsync RUs {RUs}", response.RequestCharge);
            string diseasesIdsStr = null;
            if (info.DiseasesIds.Count > 0)
            {
                diseasesIdsStr = string.Join(",", info.DiseasesIds);
            }
            var props = new Dictionary<string, string>()
            {
                { "name", info.Name},
                { "birthDate", info.BirthDate?.ToString("yyyy-MM-dd")},
                { "gender", info.Gender?.ToString() },
                { "diseasesIds", diseasesIdsStr }
            };
            var record = await CaseRecordService.CreateCaseRecordAsync(userId, medicalCase.Id, CaseRecordType.Patient, props);
            medicalCase = response.Resource;
            medicalCase.PatientInfo = record.AsPatientInfo();
            return medicalCase;
        }

        //
        //  Update
        //
        public async Task<MedicalCase> UpdateMedicalCaseAsync(string userId, string caseId, PatientInfo info)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                return await UpdateSharedMedicalCaseAsync(userId, caseId, info);
            }

            var medicalCase = await GetMedicalCaseByIdAsync(userId, caseId);
            if (medicalCase != null)
            {
                medicalCase.PatientInfo = null;
                medicalCase = await UpdateMedicalCaseAsync(medicalCase);
                string diseasesIdsStr = null;
                if (info.DiseasesIds.Count > 0)
                {
                    diseasesIdsStr = string.Join(",", info.DiseasesIds);
                }
                var props = new Dictionary<string, string>()
                {
                    { "name", info.Name},
                    { "birthDate", info.BirthDate?.ToString("yyyy-MM-dd")},
                    { "gender", info.Gender?.ToString()},
                    { "diseasesIds", diseasesIdsStr }
                };
                var record = await CaseRecordService.UpdateCaseRecordAsync(userId, medicalCase.Id, props);
                medicalCase.PatientInfo = record.AsPatientInfo();
                return medicalCase;
            }
            return null;
        }
        public async Task<MedicalCase> UpdateMedicalCaseAsync(MedicalCase medicalCase)
        {
            medicalCase.PatientInfo = null;
            medicalCase.UpdatedOn = DateTimeOffset.UtcNow;
            var response = await MedicalCases.UpsertItemAsync(medicalCase);
            Logger.LogInformation("UpdateMedicalCaseAsync RUs {RUs}", response.RequestCharge);
            return response;
        }

        //
        //  Delete
        //
        public async Task<MedicalCase> DeleteMedicalCaseAsync(string userId, string caseId, bool force = false)
        {
            var medicalCase = await GetMedicalCaseByIdAsync(userId, caseId);
            if (medicalCase != null)
            {
                if (force == true)
                {
                    var response = await MedicalCases.DeleteItemAsync<MedicalCase>(caseId, new PartitionKey(userId));
                    Logger.LogInformation("HardDeleteMedicalCaseAsync RUs {RUs}", response.RequestCharge);
                    await CaseRecordService.DeleteCaseRecordAsync(userId, caseId);
                    medicalCase = response;
                }
                else
                {
                    medicalCase.Status = CaseStatus.Deleted.ToString();
                    medicalCase = await UpdateMedicalCaseAsync(medicalCase);
                    Logger.LogInformation("SoftDeleteMedicalCaseAsync");
                }
            }
            return medicalCase;
        }
    }
}
