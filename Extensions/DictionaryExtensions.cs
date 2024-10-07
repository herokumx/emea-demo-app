using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DataCloudDemo.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueByKey<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            TValue result;
            dict.TryGetValue(key, out result);
            return result;
        }
    }
}
