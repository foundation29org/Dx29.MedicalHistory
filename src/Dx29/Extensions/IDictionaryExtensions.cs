using System;
using System.Collections.Generic;

namespace Dx29
{
    static public class IDictionaryExtensions
    {
        static public TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key)
        {
            if (dic.TryGetValue(key, out TValue value))
            {
                return value;
            }
            return default(TValue);
        }
    }
}
