using System;
using SqExpress;
using SqExpress.QueryBuilders.RecordSetter;
using SqGoods.DomainLogic.Views;
using SqExpress.Syntax.Names;
using System.Collections.Generic;

namespace SqGoods.DomainLogic.Models
{
    public record SgCategoryProductNum
    {
        public SgCategoryProductNum(Guid categoryId, int productCount)
        {
            this.CategoryId = categoryId;
            this.ProductCount = productCount;
        }

        public static SgCategoryProductNum Read(ISqDataRecordReader record, VwCategoryProductNum table)
        {
            return new SgCategoryProductNum(categoryId: table.CategoryId.Read(record), productCount: table.ProductCount.Read(record));
        }

        public static SgCategoryProductNum ReadOrdinal(ISqDataRecordReader record, VwCategoryProductNum table, int offset)
        {
            return new SgCategoryProductNum(categoryId: table.CategoryId.Read(record, offset), productCount: table.ProductCount.Read(record, offset + 1));
        }

        public Guid CategoryId { get; }

        public int ProductCount { get; }

        public static ExprColumn[] GetColumns(VwCategoryProductNum table)
        {
            return new ExprColumn[]{table.CategoryId, table.ProductCount};
        }

        public static ISqModelReader<SgCategoryProductNum, VwCategoryProductNum> GetReader()
        {
            return SgCategoryProductNumReader.Instance;
        }

        private class SgCategoryProductNumReader : ISqModelReader<SgCategoryProductNum, VwCategoryProductNum>
        {
            public static SgCategoryProductNumReader Instance { get; } = new SgCategoryProductNumReader();
            IReadOnlyList<ExprColumn> ISqModelReader<SgCategoryProductNum, VwCategoryProductNum>.GetColumns(VwCategoryProductNum table)
            {
                return SgCategoryProductNum.GetColumns(table);
            }

            SgCategoryProductNum ISqModelReader<SgCategoryProductNum, VwCategoryProductNum>.Read(ISqDataRecordReader record, VwCategoryProductNum table)
            {
                return SgCategoryProductNum.Read(record, table);
            }

            SgCategoryProductNum ISqModelReader<SgCategoryProductNum, VwCategoryProductNum>.ReadOrdinal(ISqDataRecordReader record, VwCategoryProductNum table, int offset)
            {
                return SgCategoryProductNum.ReadOrdinal(record, table, offset);
            }
        }
    }
}