using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

using Dx29.Data;

namespace Dx29.Services
{
    partial class CaseRecordService
    {
        //
        //  Get
        //
        public async Task<CaseRecord> GetCaseRecordByIdAsync(string userId, string recordId)
        {
            userId = RecordHashService.GetHash(userId);
            recordId = RecordHashService.GetHash(recordId);
            try
            {
                var response = await CaseRecords.ReadItemAsync<CaseRecord>(recordId, new PartitionKey(userId));
                Logger.LogInformation("GetCaseRecordByIdAsync RUs {RUs}", response.RequestCharge);
                return response;
            }
            catch (CosmosException ex)
            {
                if (ex.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    Logger.LogError("GetCaseRecordByIdAsync. {exception}", ex);
                    throw;
                }
                Logger.LogWarning("GetCaseRecordByIdAsync. Not found.");
            }
            catch (Exception ex)
            {
                Logger.LogError("GetCaseRecordByIdAsync. {exception}", ex);
            }
            return null;
        }

        public IAsyncEnumerable<CaseRecord> GetCaseRecordsByTypeAsync(string userId, CaseRecordType type)
        {
            userId = RecordHashService.GetHash(userId);
            return CaseRecords.GetRecordsAsync<CaseRecord>(Logger, r => r.UserId == userId && r.Type == type.ToString());
        }

        public IAsyncEnumerable<CaseRecord> GetCaseRecordsAsync(string userId)
        {
            userId = RecordHashService.GetHash(userId);
            return CaseRecords.GetRecordsAsync<CaseRecord>(Logger, r => r.UserId == userId);
        }
        public IAsyncEnumerable<CaseRecord> GetCaseRecordsAsync<TKey>(Expression<Func<CaseRecord, bool>> predicate)
        {
            return CaseRecords.GetRecordsAsync(Logger, predicate);
        }
        public IAsyncEnumerable<CaseRecord> GetCaseRecordsAsync<TKey>(Expression<Func<CaseRecord, bool>> predicate, Expression<Func<CaseRecord, TKey>> keySelector = null, bool descending = false, int skip = 0, int take = -1)
        {
            return CaseRecords.GetRecordsAsync(Logger, predicate, null, keySelector, descending, skip, take);
        }

        //
        //  Create
        //
        public async Task<CaseRecord> CreateCaseRecordAsync(string userId, string recordId, CaseRecordType type, IDictionary<string, string> properties)
        {
            userId = RecordHashService.GetHash(userId);
            recordId = RecordHashService.GetHash(recordId);
            var caseRecord = new CaseRecord(userId, recordId, type, properties);
            return await CreateCaseRecordAsync(caseRecord);
        }
        public async Task<CaseRecord> CreateCaseRecordAsync(CaseRecord caseRecord)
        {
            var response = await CaseRecords.CreateItemAsync(caseRecord);
            Logger.LogInformation("CreateCaseRecordAsync RUs {RUs}", response.RequestCharge);
            return response;
        }

        //
        //  Update
        //
        public async Task<CaseRecord> UpdateCaseRecordAsync(string userId, string recordId, IDictionary<string, string> properties)
        {
            var caseRecord = await GetCaseRecordByIdAsync(userId, recordId);
            if (caseRecord != null)
            {
                caseRecord.Properties = properties;
                return await UpsertCaseRecordAsync(caseRecord);
            }
            return null;
        }

        private async Task<CaseRecord> UpsertCaseRecordAsync(CaseRecord caseRecord)
        {
            var response = await CaseRecords.UpsertItemAsync(caseRecord);
            Logger.LogInformation("UpdateCaseRecordAsync RUs {RUs}", response.RequestCharge);
            return response;
        }

        //
        //  Delete
        //
        public async Task<CaseRecord> DeleteCaseRecordAsync(string userId, string recordId)
        {
            var caseRecord = await GetCaseRecordByIdAsync(userId, recordId);
            if (caseRecord != null)
            {
                var response = await CaseRecords.DeleteItemAsync<CaseRecord>(caseRecord.Id, new PartitionKey(caseRecord.UserId));
                Logger.LogInformation("DeleteCaseRecordAsync RUs {RUs}", response.RequestCharge);
                caseRecord = response;
            }
            return caseRecord;
        }

        public async Task DeleteCaseRecordsAsync(string userId)
        {
            var records = await GetCaseRecordsAsync(userId).ToListAsync();
            foreach (var record in records)
            {
                var response = await CaseRecords.DeleteItemAsync<CaseRecord>(record.Id, new PartitionKey(record.UserId));
                Logger.LogInformation("DeleteCaseCaseRecordAsync RUs {RUs}", response.RequestCharge);
            }
        }
    }
}
