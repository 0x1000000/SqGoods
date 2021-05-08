using System;
using SqExpress;
using SqExpress.QueryBuilders.RecordSetter;
using SqGoods.DomainLogic.Tables;
using SqExpress.Syntax.Names;
using System.Collections.Generic;

namespace SqGoods.DomainLogic.Models
{
    public record SgAttributeSetId
    {
        public SgAttributeSetId(Guid attributeSetId, Guid attributeId)
        {
            this.AttributeSetId = attributeSetId;
            this.AttributeId = attributeId;
        }

        public static SgAttributeSetId Read(ISqDataRecordReader record, TblAttributeSet table)
        {
            return new SgAttributeSetId(attributeSetId: table.AttributeSetId.Read(record), attributeId: table.AttributeId.Read(record));
        }

        public static SgAttributeSetId ReadOrdinal(ISqDataRecordReader record, TblAttributeSet table, int offset)
        {
            return new SgAttributeSetId(attributeSetId: table.AttributeSetId.Read(record, offset), attributeId: table.AttributeId.Read(record, offset + 1));
        }

        public Guid AttributeSetId { get; }

        public Guid AttributeId { get; }

        public static TableColumn[] GetColumns(TblAttributeSet table)
        {
            return new TableColumn[]{table.AttributeSetId, table.AttributeId};
        }

        public static IRecordSetterNext GetMapping(IDataMapSetter<TblAttributeSet, SgAttributeSetId> s)
        {
            return s.Set(s.Target.AttributeSetId, s.Source.AttributeSetId).Set(s.Target.AttributeId, s.Source.AttributeId);
        }

        public static IRecordSetterNext GetUpdateKeyMapping(IDataMapSetter<TblAttributeSet, SgAttributeSetId> s)
        {
            return s.Set(s.Target.AttributeSetId, s.Source.AttributeSetId);
        }

        public static IRecordSetterNext GetUpdateMapping(IDataMapSetter<TblAttributeSet, SgAttributeSetId> s)
        {
            return s.Set(s.Target.AttributeId, s.Source.AttributeId);
        }

        public static ISqModelReader<SgAttributeSetId, TblAttributeSet> GetReader()
        {
            return SgAttributeSetIdReader.Instance;
        }

        private class SgAttributeSetIdReader : ISqModelReader<SgAttributeSetId, TblAttributeSet>
        {
            public static SgAttributeSetIdReader Instance { get; } = new SgAttributeSetIdReader();
            IReadOnlyList<ExprColumn> ISqModelReader<SgAttributeSetId, TblAttributeSet>.GetColumns(TblAttributeSet table)
            {
                return SgAttributeSetId.GetColumns(table);
            }

            SgAttributeSetId ISqModelReader<SgAttributeSetId, TblAttributeSet>.Read(ISqDataRecordReader record, TblAttributeSet table)
            {
                return SgAttributeSetId.Read(record, table);
            }

            SgAttributeSetId ISqModelReader<SgAttributeSetId, TblAttributeSet>.ReadOrdinal(ISqDataRecordReader record, TblAttributeSet table, int offset)
            {
                return SgAttributeSetId.ReadOrdinal(record, table, offset);
            }
        }

        public static ISqModelUpdaterKey<SgAttributeSetId, TblAttributeSet> GetUpdater()
        {
            return SgAttributeSetIdUpdater.Instance;
        }

        private class SgAttributeSetIdUpdater : ISqModelUpdaterKey<SgAttributeSetId, TblAttributeSet>
        {
            public static SgAttributeSetIdUpdater Instance { get; } = new SgAttributeSetIdUpdater();
            IRecordSetterNext ISqModelUpdater<SgAttributeSetId, TblAttributeSet>.GetMapping(IDataMapSetter<TblAttributeSet, SgAttributeSetId> dataMapSetter)
            {
                return SgAttributeSetId.GetMapping(dataMapSetter);
            }

            IRecordSetterNext ISqModelUpdaterKey<SgAttributeSetId, TblAttributeSet>.GetUpdateKeyMapping(IDataMapSetter<TblAttributeSet, SgAttributeSetId> dataMapSetter)
            {
                return SgAttributeSetId.GetUpdateKeyMapping(dataMapSetter);
            }

            IRecordSetterNext ISqModelUpdaterKey<SgAttributeSetId, TblAttributeSet>.GetUpdateMapping(IDataMapSetter<TblAttributeSet, SgAttributeSetId> dataMapSetter)
            {
                return SgAttributeSetId.GetUpdateMapping(dataMapSetter);
            }
        }
    }
}