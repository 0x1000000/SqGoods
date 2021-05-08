using System;
using SqExpress;
using SqExpress.QueryBuilders.RecordSetter;
using SqGoods.DomainLogic.Tables;
using SqExpress.Syntax.Names;
using System.Collections.Generic;

namespace SqGoods.DomainLogic.Models
{
    public record SgProductAttributeSet
    {
        public SgProductAttributeSet(Guid productId, Guid attributeSetId)
        {
            this.ProductId = productId;
            this.AttributeSetId = attributeSetId;
        }

        public static SgProductAttributeSet Read(ISqDataRecordReader record, TblProductAttributeSet table)
        {
            return new SgProductAttributeSet(productId: table.ProductId.Read(record), attributeSetId: table.AttributeSetId.Read(record));
        }

        public static SgProductAttributeSet ReadOrdinal(ISqDataRecordReader record, TblProductAttributeSet table, int offset)
        {
            return new SgProductAttributeSet(productId: table.ProductId.Read(record, offset), attributeSetId: table.AttributeSetId.Read(record, offset + 1));
        }

        public Guid ProductId { get; }

        public Guid AttributeSetId { get; }

        public static TableColumn[] GetColumns(TblProductAttributeSet table)
        {
            return new TableColumn[]{table.ProductId, table.AttributeSetId};
        }

        public static IRecordSetterNext GetMapping(IDataMapSetter<TblProductAttributeSet, SgProductAttributeSet> s)
        {
            return s.Set(s.Target.ProductId, s.Source.ProductId).Set(s.Target.AttributeSetId, s.Source.AttributeSetId);
        }

        public static ISqModelReader<SgProductAttributeSet, TblProductAttributeSet> GetReader()
        {
            return SgProductAttributeSetReader.Instance;
        }

        private class SgProductAttributeSetReader : ISqModelReader<SgProductAttributeSet, TblProductAttributeSet>
        {
            public static SgProductAttributeSetReader Instance { get; } = new SgProductAttributeSetReader();
            IReadOnlyList<ExprColumn> ISqModelReader<SgProductAttributeSet, TblProductAttributeSet>.GetColumns(TblProductAttributeSet table)
            {
                return SgProductAttributeSet.GetColumns(table);
            }

            SgProductAttributeSet ISqModelReader<SgProductAttributeSet, TblProductAttributeSet>.Read(ISqDataRecordReader record, TblProductAttributeSet table)
            {
                return SgProductAttributeSet.Read(record, table);
            }

            SgProductAttributeSet ISqModelReader<SgProductAttributeSet, TblProductAttributeSet>.ReadOrdinal(ISqDataRecordReader record, TblProductAttributeSet table, int offset)
            {
                return SgProductAttributeSet.ReadOrdinal(record, table, offset);
            }
        }

        public static ISqModelUpdater<SgProductAttributeSet, TblProductAttributeSet> GetUpdater()
        {
            return SgProductAttributeSetUpdater.Instance;
        }

        private class SgProductAttributeSetUpdater : ISqModelUpdater<SgProductAttributeSet, TblProductAttributeSet>
        {
            public static SgProductAttributeSetUpdater Instance { get; } = new SgProductAttributeSetUpdater();
            IRecordSetterNext ISqModelUpdater<SgProductAttributeSet, TblProductAttributeSet>.GetMapping(IDataMapSetter<TblProductAttributeSet, SgProductAttributeSet> dataMapSetter)
            {
                return SgProductAttributeSet.GetMapping(dataMapSetter);
            }
        }
    }
}