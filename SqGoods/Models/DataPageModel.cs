using System;
using System.Collections.Generic;
using SqExpress;

namespace SqGoods.Models
{
    public class DataPageModel<T>
    {
        public static readonly DataPageModel<T> Empty = new(Array.Empty<T>(), 0, 0);

        public DataPageModel(IReadOnlyList<T> items, int offset, int total)
        {
            this.Items = items;
            this.Offset = offset;
            this.Total = total;
        }

        public IReadOnlyList<T> Items { get; }

        public int Offset { get; }

        public int Total { get; }


        public static implicit operator DataPageModel<T>(DataPage<T> p) 
            => new DataPageModel<T>(p.Items, p.Offset, p.Total);
    }
}