using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dx29
{
    static public class EnumerableExtensions
    {
        public static async Task<List<TSource>> ToListAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
        {
            var list = new List<TSource>();
            await foreach (var item in source.WithCancellation(cancellationToken))
            {
                list.Add(item);
            }
            return list;
        }

        public static async Task<TSource> FirstOrDefaultAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
        {
            await foreach (var item in source.WithCancellation(cancellationToken))
            {
                return item;
            }
            return default(TSource);
        }
    }
}
