using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SqExpress;
using SqExpress.DataAccess;
using SqExpress.ModelSelect;

namespace SqGoods.Infrastructure
{
    public static class Utils
    {
        public static IReadOnlyList<TRes> SelectReadOnlyList<T, TRes>(this IReadOnlyList<T> source, Func<T, TRes> mapper)
        {
            var result = new TRes[source.Count];

            for (int i = 0; i < source.Count; i++)
            {
                result[i] = mapper(source[i]);
            }

            return result;
        }
    }
}