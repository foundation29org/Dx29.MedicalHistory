using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;

namespace Dx29.Services
{
    static public class DatabaseExtensions
    {
        static public async Task<bool> RecordExistsAsync(this Container container, string id, PartitionKey partitionKey)
        {
            using (var responseMessage = await container.ReadItemStreamAsync(id, partitionKey))
            {
                return responseMessage.IsSuccessStatusCode;
            }
        }

        static public IAsyncEnumerable<TRecord> ExecuteQueryAsync<TRecord>(this Database database, ILogger logger, string container, string query)
        {
            return ExecuteQueryAsync<TRecord>(database.GetContainer(container), logger, query);
        }
        static public IAsyncEnumerable<TRecord> ExecuteQueryAsync<TRecord>(this Container container, ILogger logger, string query)
        {
            var queryDef = new QueryDefinition(query);
            var iterator = container.GetItemQueryIterator<TRecord>(queryDef);
            return EnumRecordsAsync(logger, iterator);
        }

        static public async Task<TRecord> GetFirstOrDefaultRecordAsync<TRecord>(this Container container, ILogger logger, Expression<Func<TRecord, bool>> predicate)
        {
            var queryable = container.GetItemLinqQueryable<TRecord>().Where(predicate).Take(1);

            var iterator = queryable.ToFeedIterator();
            var records = EnumRecordsAsync(logger, iterator);
            await foreach (var item in records)
            {
                return item;
            }
            return default(TRecord);
        }

        static public IAsyncEnumerable<TRecord> GetRecordsAsync<TRecord>(this Container container, ILogger logger, Expression<Func<TRecord, bool>> predicate1, Expression<Func<TRecord, bool>> predicate2 = null)
        {
            var queryable = container.GetItemLinqQueryable<TRecord>().Where(predicate1);
            if (predicate2 != null)
            {
                queryable = queryable.Where(predicate2);
            }

            var iterator = queryable.ToFeedIterator();
            return EnumRecordsAsync(logger, iterator);
        }
        static public IAsyncEnumerable<TRecord> GetRecordsAsync<TRecord, TKey>(this Container container, ILogger logger, Expression<Func<TRecord, bool>> predicate1, Expression<Func<TRecord, bool>> predicate2, Expression<Func<TRecord, TKey>> keySelector = null, bool descending = false, int skip = 0, int take = -1)
        {
            var queryable = container.GetItemLinqQueryable<TRecord>().Where(predicate1);
            if (predicate2 != null)
            {
                queryable = queryable.Where(predicate2);
            }
            if (keySelector != null)
            {
                queryable = descending ? queryable.OrderByDescending(keySelector) : queryable.OrderBy(keySelector);
            }
            if (skip > 0)
            {
                queryable = queryable.Skip(skip);
            }
            if (take > -1)
            {
                queryable = queryable.Take(take);
            }

            var iterator = queryable.ToFeedIterator();
            return EnumRecordsAsync(logger, iterator);
        }

        static public async IAsyncEnumerable<TRecord> EnumRecordsAsync<TRecord>(ILogger logger, FeedIterator<TRecord> iterator)
        {
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                logger.LogInformation("EnumRecordsAsync RUs {RUs}", response.RequestCharge);
                foreach (var item in response)
                {
                    yield return item;
                }
            }
        }
    }
}
