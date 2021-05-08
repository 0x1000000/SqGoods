using System;
using SqExpress;
using SqExpress.QueryBuilders.RecordSetter;
using SqGoods.DomainLogic.Views;
using SqExpress.Syntax.Names;
using System.Collections.Generic;

namespace SqGoods.DomainLogic.Models
{
    public record SgAttributeCatsNum
    {
        public SgAttributeCatsNum(Guid attributeId, int categoriesCount)
        {
            this.AttributeId = attributeId;
            this.CategoriesCount = categoriesCount;
        }

        public static SgAttributeCatsNum Read(ISqDataRecordReader record, VwAttributeCatsNum table)
        {
            return new SgAttributeCatsNum(attributeId: table.AttributeId.Read(record), categoriesCount: table.CategoriesCount.Read(record));
        }

        public static SgAttributeCatsNum ReadOrdinal(ISqDataRecordReader record, VwAttributeCatsNum table, int offset)
        {
            return new SgAttributeCatsNum(attributeId: table.AttributeId.Read(record, offset), categoriesCount: table.CategoriesCount.Read(record, offset + 1));
        }

        public Guid AttributeId { get; }

        public int CategoriesCount { get; }

        public static ExprColumn[] GetColumns(VwAttributeCatsNum table)
        {
            return new ExprColumn[]{table.AttributeId, table.CategoriesCount};
        }

        public static ISqModelReader<SgAttributeCatsNum, VwAttributeCatsNum> GetReader()
        {
            return SgAttributeCatsNumReader.Instance;
        }

        private class SgAttributeCatsNumReader : ISqModelReader<SgAttributeCatsNum, VwAttributeCatsNum>
        {
            public static SgAttributeCatsNumReader Instance { get; } = new SgAttributeCatsNumReader();
            IReadOnlyList<ExprColumn> ISqModelReader<SgAttributeCatsNum, VwAttributeCatsNum>.GetColumns(VwAttributeCatsNum table)
            {
                return SgAttributeCatsNum.GetColumns(table);
            }

            SgAttributeCatsNum ISqModelReader<SgAttributeCatsNum, VwAttributeCatsNum>.Read(ISqDataRecordReader record, VwAttributeCatsNum table)
            {
                return SgAttributeCatsNum.Read(record, table);
            }

            SgAttributeCatsNum ISqModelReader<SgAttributeCatsNum, VwAttributeCatsNum>.ReadOrdinal(ISqDataRecordReader record, VwAttributeCatsNum table, int offset)
            {
                return SgAttributeCatsNum.ReadOrdinal(record, table, offset);
            }
        }
    }
}