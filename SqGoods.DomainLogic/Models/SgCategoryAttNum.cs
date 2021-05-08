using System;
using SqExpress;
using SqGoods.DomainLogic.Views;
using SqExpress.Syntax.Names;
using System.Collections.Generic;
using SqExpress.QueryBuilders.RecordSetter;

namespace SqGoods.DomainLogic.Models
{
    public record SgCategoryAttNum
    {
        public SgCategoryAttNum(Guid categoryId, int attributesCount)
        {
            this.CategoryId = categoryId;
            this.AttributesCount = attributesCount;
        }

        public static SgCategoryAttNum Read(ISqDataRecordReader record, VwCategoryAttNum table)
        {
            return new SgCategoryAttNum(categoryId: table.CategoryId.Read(record), attributesCount: table.AttributesCount.Read(record));
        }

        public static SgCategoryAttNum ReadOrdinal(ISqDataRecordReader record, VwCategoryAttNum table, int offset)
        {
            return new SgCategoryAttNum(categoryId: table.CategoryId.Read(record, offset), attributesCount: table.AttributesCount.Read(record, offset + 1));
        }

        public Guid CategoryId { get; }

        public int AttributesCount { get; }

        public static ExprColumn[] GetColumns(VwCategoryAttNum table)
        {
            return new ExprColumn[]{table.CategoryId, table.AttributesCount};
        }

        public static ISqModelReader<SgCategoryAttNum, VwCategoryAttNum> GetReader()
        {
            return SgCategoryAttNumReader.Instance;
        }

        private class SgCategoryAttNumReader : ISqModelReader<SgCategoryAttNum, VwCategoryAttNum>
        {
            public static SgCategoryAttNumReader Instance { get; } = new SgCategoryAttNumReader();
            IReadOnlyList<ExprColumn> ISqModelReader<SgCategoryAttNum, VwCategoryAttNum>.GetColumns(VwCategoryAttNum table)
            {
                return SgCategoryAttNum.GetColumns(table);
            }

            SgCategoryAttNum ISqModelReader<SgCategoryAttNum, VwCategoryAttNum>.Read(ISqDataRecordReader record, VwCategoryAttNum table)
            {
                return SgCategoryAttNum.Read(record, table);
            }

            SgCategoryAttNum ISqModelReader<SgCategoryAttNum, VwCategoryAttNum>.ReadOrdinal(ISqDataRecordReader record, VwCategoryAttNum table, int offset)
            {
                return SgCategoryAttNum.ReadOrdinal(record, table, offset);
            }
        }

        private class SgCategoryStatReader : ISqModelReader<SgCategoryAttNum, VwCategoryAttNum>
        {
            public static SgCategoryStatReader Instance { get; } = new SgCategoryStatReader();
            IReadOnlyList<ExprColumn> ISqModelReader<SgCategoryAttNum, VwCategoryAttNum>.GetColumns(VwCategoryAttNum table)
            {
                return SgCategoryAttNum.GetColumns(table);
            }

            SgCategoryAttNum ISqModelReader<SgCategoryAttNum, VwCategoryAttNum>.Read(ISqDataRecordReader record, VwCategoryAttNum table)
            {
                return SgCategoryAttNum.Read(record, table);
            }

            SgCategoryAttNum ISqModelReader<SgCategoryAttNum, VwCategoryAttNum>.ReadOrdinal(ISqDataRecordReader record, VwCategoryAttNum table, int offset)
            {
                return SgCategoryAttNum.ReadOrdinal(record, table, offset);
            }
        }
    }
}