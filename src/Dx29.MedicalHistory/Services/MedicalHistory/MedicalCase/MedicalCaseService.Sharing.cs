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
        //  Get
        //
        private async Task<MedicalCase> GetSharedMedicalCaseByIdAsync(string userId, string caseId, bool checkStatus = true)
        {
            try
            {
                var response = await MedicalCases.ReadItemAsync<MedicalCase>(caseId, new PartitionKey(userId));
                Logger.LogInformation("GetSharedMedicalCaseByIdAsync RUs {RUs}", response.RequestCharge);
                return await MergeSharedMedicalCaseAsync(response, checkStatus);
            }
            catch (CosmosException ex)
            {
                if (ex.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    Logger.LogError("GetSharedMedicalCaseByIdAsync. {exception}", ex);
                    throw;
                }
                Logger.LogWarning("GetSharedMedicalCaseByIdAsync. Not found.");
            }
            catch (Exception ex)
            {
                Logger.LogError("GetSharedMedicalCaseByIdAsync. {exception}", ex);
            }
            return null;
        }

        //
        //  Get SharedBy
        //
        public async Task<SharedBy> GetSharedByAsync(string userId, string caseId)
        {
            var medicalCase = await GetRawMedicalCaseByIdAsync(userId, caseId);
            if (medicalCase != null)
            {
                return GetSharedByInfo(medicalCase);
            }
            return null;
        }

        //
        //  Share
        //
        public async Task<MedicalCase> ShareMedicalCaseAsync(string userId, string caseId, string targetEmail)
        {
            targetEmail = targetEmail.ToLower();
            string targetUserId = AccountHashService.GetHash(targetEmail);

            // Get sharing case
            var sharingCase = await GetRawMedicalCaseByIdAsync(userId, caseId);
            if (sharingCase != null)
            {
                var mainCase = await GetMainMedicalCaseAsync(sharingCase);
                if (mainCase != null)
                {
                    (var ok, var message) = CheckMedicalCaseCanBeShared(mainCase, sharingCase, targetEmail);
                    if (ok)
                    {
                        // Create shared case
                        var medicalCase = new MedicalCase(targetUserId, shared: true);
                        if (sharingCase.Id == mainCase.Id)
                        {
                            var sharedBy = new SharedBy(userId, caseId, SharedByStatus.Accepted);
                            medicalCase.SharedBy.Add(sharedBy);
                        }
                        else
                        {
                            medicalCase.Status = CaseStatus.PendingShared.ToString();
                            var sharedBy = new SharedBy(userId, caseId, SharedByStatus.Accepted);
                            medicalCase.SharedBy.Add(sharedBy);
                        }
                        if (sharingCase.Id.StartsWith('s'))
                        {
                            medicalCase.SharedBy.Add(sharingCase.SharedBy.Where(r => r.CaseId.StartsWith('c')).First());
                        }
                        var response = await MedicalCases.CreateItemAsync(medicalCase);
                        Logger.LogInformation("ShareMedicalCaseAsync RUs {RUs}", response.RequestCharge);
                        medicalCase = response.Resource;

                        // Assign to Main case
                        mainCase.Status = CaseStatus.Sharing.ToString();
                        var sharedWith = new SharedWith(targetEmail, SharedWithStatus.Pending);
                        mainCase.SharedWith.Add(sharedWith);
                        if (mainCase.Id == sharingCase.Id)
                        {
                            sharedWith.Status = SharedWithStatus.Shared.ToString();
                        }
                        else
                        {
                            // Assign to Sharing case
                            sharedWith = new SharedWith(targetEmail, SharedWithStatus.Pending);
                            sharingCase.SharedWith.Add(sharedWith);
                            await UpdateMedicalCaseAsync(sharingCase);
                        }
                        await UpdateMedicalCaseAsync(mainCase);

                        return medicalCase;
                    }
                    else
                    {
                        throw new InvalidOperationException(message);
                    }
                }
            }
            return null;
        }

        private (bool, string) CheckMedicalCaseCanBeShared(MedicalCase mainCase, MedicalCase sharingCase, string targetEmail)
        {
            var targetId = AccountHashService.GetHash(targetEmail);

            // Check sharing case
            if (sharingCase.Status == CaseStatus.Deleted.ToString())
            {
                return (false, "Medical Case is deleted.");
            }
            if (sharingCase.UserId == targetId)
            {
                return (false, "Cannot share a medical case with yourself.");
            }
            if (sharingCase.Status == CaseStatus.PendingShared.ToString())
            {
                return (false, "Medical case awaiting approval.");
            }

            // Check main case
            if (mainCase.Status == CaseStatus.Deleted.ToString())
            {
                return (false, "Medical Case is deleted.");
            }
            if (mainCase.UserId == targetId)
            {
                return (false, "Cannot share a medical case with the case owner.");
            }
            if (mainCase.SharedWith.Any(r => r.UserId.EqualsNoCase(targetEmail)))
            {
                return (false, "This medical case is already shared with this user.");
            }

            return (true, null);
        }

        public async Task<MedicalCase> AcceptSharingMedicalCaseAsync(string userId, string caseId, string targetEmail)
        {
            if (!caseId.StartsWith('c'))
            {
                throw new InvalidOperationException("Only owner can accept sharing this case.");
            }

            targetEmail = targetEmail.ToLower();
            string targetUserId = AccountHashService.GetHash(targetEmail);

            // Get sharing case
            var sharingCase = await GetRawMedicalCaseByIdAsync(userId, caseId);
            if (sharingCase != null)
            {
                // Update SharedWith
                var sharedWith = sharingCase.SharedWith.Where(r => r.UserId.EqualsNoCase(targetEmail)).FirstOrDefault();
                if (sharedWith != null)
                {
                    sharedWith.Status = SharedWithStatus.Shared.ToString();
                }
                await UpdateMedicalCaseAsync(sharingCase);

                // Update SharedBy
                var medicalCases = await GetRawMedicalCasesAsync(targetUserId, includeDeleted: true);
                foreach (var medicalCase in medicalCases)
                {
                    var sharedBy = medicalCase.SharedBy.Where(r => r.UserId == userId && r.CaseId == caseId).FirstOrDefault();
                    if (sharedBy != null)
                    {
                        medicalCase.Status = CaseStatus.Shared.ToString();
                        sharedBy.Status = SharedByStatus.Accepted.ToString();
                        await UpdateMedicalCaseAsync(medicalCase);

                        // If shared by 3th, update 3th case
                        var sharer = medicalCase.SharedBy.Where(r => r.CaseId.StartsWith('s')).FirstOrDefault();
                        if (sharer != null)
                        {
                            var sharerCase = await GetRawMedicalCaseByIdAsync(sharer.UserId, sharer.CaseId);
                            if (sharerCase != null)
                            {
                                sharedWith = sharerCase.SharedWith.Where(r => r.UserId.EqualsNoCase(targetEmail)).FirstOrDefault();
                                if (sharedWith != null)
                                {
                                    sharedWith.Status = SharedWithStatus.Shared.ToString();
                                }
                                await UpdateMedicalCaseAsync(sharerCase);
                            }
                        }
                        return medicalCase;
                    }
                }
            }
            return null;
        }

        public async Task<MedicalCase> RevokeSharingMedicalCaseAsync(string userId, string caseId, string targetEmail)
        {
            targetEmail = targetEmail.ToLower();
            string targetUserId = AccountHashService.GetHash(targetEmail);

            // Get sharing case
            var sharingCase = await GetRawMedicalCaseByIdAsync(userId, caseId);
            if (sharingCase != null)
            {
                // Update SharedWith
                var sharedWith = sharingCase.SharedWith.Where(r => r.UserId.EqualsNoCase(targetEmail)).FirstOrDefault();
                if (sharedWith != null)
                {
                    sharedWith.Status = SharedWithStatus.Revoked.ToString();
                }
                await UpdateMedicalCaseAsync(sharingCase);

                // Update SharedBy
                var medicalCases = await GetRawMedicalCasesAsync(targetUserId, includeDeleted: true);
                foreach (var medicalCase in medicalCases)
                {
                    var sharedBy = medicalCase.SharedBy.Where(r => r.UserId == userId && r.CaseId == caseId).FirstOrDefault();
                    if (sharedBy != null)
                    {
                        medicalCase.Status = CaseStatus.StopShared.ToString();
                        await UpdateMedicalCaseAsync(medicalCase);

                        // If shared by 3th, update 3th case
                        var sharer = medicalCase.SharedBy.Where(r => r.CaseId.StartsWith('s')).FirstOrDefault();
                        if (sharer != null)
                        {
                            var sharerCase = await GetRawMedicalCaseByIdAsync(sharer.UserId, sharer.CaseId);
                            if (sharerCase != null)
                            {
                                sharedWith = sharerCase.SharedWith.Where(r => r.UserId.EqualsNoCase(targetEmail)).FirstOrDefault();
                                if (sharedWith != null)
                                {
                                    sharedWith.Status = SharedWithStatus.Revoked.ToString();
                                }
                                await UpdateMedicalCaseAsync(sharerCase);
                            }
                        }
                        return medicalCase;
                    }
                }
            }
            return null;
        }

        public async Task<MedicalCase> DeleteSharingMedicalCaseAsync(string userId, string caseId, string targetEmail)
        {
            targetEmail = targetEmail.ToLower();
            string targetUserId = AccountHashService.GetHash(targetEmail);

            // Get sharing case
            var sharingCase = await GetRawMedicalCaseByIdAsync(userId, caseId);
            if (sharingCase != null)
            {
                var mainCase = await GetMainMedicalCaseAsync(sharingCase);
                if (mainCase != null)
                {
                    // Remove main SharedWith
                    var sharedWith = mainCase.SharedWith.Where(r => r.UserId.EqualsNoCase(targetEmail)).FirstOrDefault();
                    if (sharedWith != null)
                    {
                        mainCase.SharedWith.Remove(sharedWith);
                        await UpdateMedicalCaseAsync(mainCase);
                    }

                    // Remove sharer SharedWith
                    if (sharingCase.Id != mainCase.Id)
                    {
                        sharedWith = sharingCase.SharedWith.Where(r => r.UserId.EqualsNoCase(targetEmail)).FirstOrDefault();
                        if (sharedWith != null)
                        {
                            sharingCase.SharedWith.Remove(sharedWith);
                            await UpdateMedicalCaseAsync(sharingCase);
                        }
                    }

                    // Delete target case
                    var medicalCases = await GetRawMedicalCasesAsync(targetUserId, includeDeleted: true);
                    foreach (var medicalCase in medicalCases)
                    {
                        var sharedBy = medicalCase.SharedBy.Where(r => r.UserId == userId && r.CaseId == caseId).FirstOrDefault();
                        if (sharedBy != null)
                        {
                            // medicalCase is target case
                            medicalCase.Status = CaseStatus.Deleted.ToString();
                            await UpdateMedicalCaseAsync(medicalCase);
                            // Remove SharedWith in parent case
                            sharedBy = medicalCase.SharedBy.Where(r => r.CaseId.StartsWith('s')).FirstOrDefault();
                            if (sharedBy != null)
                            {
                                var parentCase = await GetRawMedicalCaseByIdAsync(sharedBy.UserId, sharedBy.CaseId);
                                if (parentCase != null)
                                {
                                    sharedWith = parentCase.SharedWith.Where(r => r.UserId.EqualsNoCase(targetEmail)).FirstOrDefault();
                                    if (sharedWith != null)
                                    {
                                        parentCase.SharedWith.Remove(sharedWith);
                                        await UpdateMedicalCaseAsync(parentCase);
                                    }
                                }
                            }
                            // Remove SharedBy in cases shared by target
                            foreach (var shWith in medicalCase.SharedWith)
                            {
                                var mcases = await GetRawMedicalCasesAsync(AccountHashService.GetHash(shWith.UserId), includeDeleted: true);
                                foreach (var mcase in mcases)
                                {
                                    var shBy = mcase.SharedBy.Where(r => r.UserId == medicalCase.UserId && r.CaseId == medicalCase.Id).FirstOrDefault();
                                    if (shBy != null)
                                    {
                                        mcase.SharedBy.Remove(shBy);
                                        await UpdateMedicalCaseAsync(mcase);
                                        break;
                                    }
                                }
                            }

                            return medicalCase;
                        }
                    }
                }
            }
            return null;
        }

        public async Task StopSharingMedicalCaseAsync(string userId, string caseId, string targetEmail)
        {
            targetEmail = targetEmail.ToLower();
            string targetUserId = AccountHashService.GetHash(targetEmail);

            // Get sharing case
            var sharingCase = await GetRawMedicalCaseByIdAsync(userId, caseId);
            if (sharingCase != null)
            {
                // Remove SharedWith
                var sharedWith = GetSharedWithInfo(targetUserId, sharingCase);
                if (sharedWith != null)
                {
                    sharingCase.SharedWith.Remove(sharedWith);
                }
                await UpdateMedicalCaseAsync(sharingCase);

                // Remove SharedBy
                var medicalCases = await GetRawMedicalCasesAsync(targetUserId, includeDeleted: true);
                foreach (var medicalCase in medicalCases)
                {
                    var sharedBy = medicalCase.SharedBy.Where(r => r.UserId == userId && r.CaseId == caseId).FirstOrDefault();
                    if (sharedBy != null)
                    {
                        medicalCase.Status = CaseStatus.StopShared.ToString();
                        medicalCase.SharedBy.Remove(sharedBy);
                        await UpdateMedicalCaseAsync(medicalCase);
                        break;
                    }
                }
                await UpdateMedicalCaseAsync(sharingCase);
            }
        }

        //
        //  Update
        //
        private async Task<MedicalCase> UpdateSharedMedicalCaseAsync(string userId, string caseId, PatientInfo info)
        {
            var medicalCase = await GetMedicalCaseByIdAsync(userId, caseId);
            if (medicalCase != null)
            {
                var sharedBy = GetSharedByInfo(medicalCase);
                if (sharedBy != null)
                {
                    var sharingCase = await GetMedicalCaseByIdAsync(sharedBy.UserId, sharedBy.CaseId);
                    if (CheckSharedWithStatus(medicalCase.UserId, sharingCase))
                    {
                        var props = new Dictionary<string, string>()
                        {
                            { "name", info.Name},
                            { "birthDate", info.BirthDate?.ToString("yyyy-MM-dd")},
                            { "gender", info.Gender?.ToString()},
                        };
                        var record = await CaseRecordService.UpdateCaseRecordAsync(sharedBy.UserId, sharedBy.CaseId, props);
                        medicalCase.PatientInfo = record.AsPatientInfo();
                        return medicalCase;
                    }
                }
            }
            return null;
        }

        //
        //  Helpers
        //
        private async Task<MedicalCase> MergeSharedMedicalCaseAsync(MedicalCase medicalCase, bool checkStatus = true)
        {
            var sharedBy = GetSharedByInfo(medicalCase);
            if (sharedBy != null)
            {
                var sharingCase = await GetMedicalCaseByIdAsync(sharedBy.UserId, sharedBy.CaseId);
                if (sharingCase != null)
                {
                    if (!checkStatus || CheckSharedWithStatus(medicalCase.UserId, sharingCase))
                    {
                        medicalCase.PatientInfo = sharingCase.PatientInfo;
                        medicalCase.ResourceGroups = sharingCase.ResourceGroups;
                        return medicalCase;
                    }
                }
                else
                {
                    return medicalCase;
                }
            }
            return null;
        }

        private SharedBy GetSharedByInfo(MedicalCase medicalCase)
        {
            var sharedBy = medicalCase.SharedBy.Where(r => r.CaseId.StartsWith('c')).FirstOrDefault();
            if (sharedBy != null)
            {
                if (sharedBy.Status == SharedByStatus.Accepted.ToString())
                {
                    return sharedBy;
                }
            }
            return null;
        }

        private bool CheckSharedWithStatus(string userId, MedicalCase medicalCase)
        {
            var sharedWith = GetSharedWithInfo(userId, medicalCase);
            if (sharedWith.Status == SharedWithStatus.Shared.ToString())
            {
                return true;
            }
            return false;
        }

        private SharedWith GetSharedWithInfo(string userId, MedicalCase medicalCase)
        {
            return medicalCase.SharedWith.Where(r => AccountHashService.GetHash(r.UserId) == userId).FirstOrDefault();
        }
    }
}
