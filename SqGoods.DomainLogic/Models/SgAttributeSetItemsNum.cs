using System;
using SqExpress;
using SqExpress.QueryBuilders.RecordSetter;
using SqGoods.DomainLogic.Views;
using SqExpress.Syntax.Names;
using System.Collections.Generic;

namespace SqGoods.DomainLogic.Models
{
    public record SgAttributeSetItemsNum
    {
        public SgAttributeSetItemsNum(Guid attributeId, int setItemsNum)
        {
            this.AttributeId = attributeId;
            this.SetItemsNum = setItemsNum;
        }

        public static SgAttributeSetItemsNum Read(ISqDataRecordReader record, VwAttributeSetItemsNum table)
        {
            return new SgAttributeSetItemsNum(attributeId: table.AttributeId.Read(record), setItemsNum: table.SetItemsNum.Read(record));
        }

        public static SgAttributeSetItemsNum ReadOrdinal(ISqDataRecordReader record, VwAttributeSetItemsNum table, int offset)
        {
            return new SgAttributeSetItemsNum(attributeId: table.AttributeId.Read(record, offset), setItemsNum: table.SetItemsNum.Read(record, offset + 1));
        }

        public Guid AttributeId { get; }

        public int SetItemsNum { get; }

        public static ExprColumn[] GetColumns(VwAttributeSetItemsNum table)
        {
            return new ExprColumn[]{table.AttributeId, table.SetItemsNum};
        }

        public static ISqModelReader<SgAttributeSetItemsNum, VwAttributeSetItemsNum> GetReader()
        {
            return SgAttributeSetItemsNumReader.Instance;
        }

        private class SgAttributeSetItemsNumReader : ISqModelReader<SgAttributeSetItemsNum, VwAttributeSetItemsNum>
        {
            public static SgAttributeSetItemsNumReader Instance { get; } = new SgAttributeSetItemsNumReader();
            IReadOnlyList<ExprColumn> ISqModelReader<SgAttributeSetItemsNum, VwAttributeSetItemsNum>.GetColumns(VwAttributeSetItemsNum table)
            {
                return SgAttributeSetItemsNum.GetColumns(table);
            }

            SgAttributeSetItemsNum ISqModelReader<SgAttributeSetItemsNum, VwAttributeSetItemsNum>.Read(ISqDataRecordReader record, VwAttributeSetItemsNum table)
            {
                return SgAttributeSetItemsNum.Read(record, table);
            }

            SgAttributeSetItemsNum ISqModelReader<SgAttributeSetItemsNum, VwAttributeSetItemsNum>.ReadOrdinal(ISqDataRecordReader record, VwAttributeSetItemsNum table, int offset)
            {
                return SgAttributeSetItemsNum.ReadOrdinal(record, table, offset);
            }
        }
    }
}