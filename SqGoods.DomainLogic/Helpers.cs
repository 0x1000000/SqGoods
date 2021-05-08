using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SqExpress;
using SqExpress.DataAccess;
using SqExpress.ModelSelect;

namespace SqGoods.DomainLogic
{
    public static class Helpers
    {
        public static Dictionary<TKey, TValue> Append<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
            where TKey : notnull
        {
            if (!dictionary.TryAdd(key, value))
            {
                throw new Exception($"Key duplication: {key}");
            }
            return dictionary;
        }

        public static Dictionary<TKey, ICollection<TValue>> AppendGroup<TKey, TValue>(this Dictionary<TKey, ICollection<TValue>> dictionary, TKey key, TValue value)
            where TKey : notnull
        {
            if (!dictionary.TryGetValue(key, out var list))
            {
                list = new List<TValue>();
                dictionary.Add(key, list);
            }

            list.Add(value);
            return dictionary;
        }

        public static Task<Dictionary<TKey, TValue>> QueryDict<TRes, TKey, TValue>(
            this ModelRequestData<TRes> modelRequestData,
            ISqDatabase database,
            Func<TRes, (TKey Key, TValue Value)> keySplitter)
            where TKey : notnull
            =>
            modelRequestData.Query(database,
                new Dictionary<TKey, TValue>(),
                (acc, next) =>
                {
                    var kv = keySplitter(next);
                    return acc.Append(kv.Key, kv.Value);
                });

        public static Task<Dictionary<TKey, ICollection<TValue>>> QueryDictGroup<TRes, TKey, TValue>(
            this ModelRequestData<TRes> modelRequestData,
            ISqDatabase database,
            Func<TRes, (TKey Key, TValue Value)> keySplitter)
            where TKey : notnull
            =>
            modelRequestData.Query(database,
                new Dictionary<TKey, ICollection<TValue>>(),
                (acc, next) =>
                {
                    var kv = keySplitter(next);
                    return acc.AppendGroup(kv.Key, kv.Value);
                });
    }
}