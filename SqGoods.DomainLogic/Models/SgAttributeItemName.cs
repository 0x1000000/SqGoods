using System;
using SqExpress;
using SqExpress.QueryBuilders.RecordSetter;
using SqGoods.DomainLogic.Tables;
using SqExpress.Syntax.Names;
using System.Collections.Generic;

namespace SqGoods.DomainLogic.Models
{
    public record SgAttributeItemName
    {
        public SgAttributeItemName(Guid attributeSetId, string name)
        {
            this.AttributeSetId = attributeSetId;
            this.Name = name;
        }

        public static SgAttributeItemName Read(ISqDataRecordReader record, TblAttributeSet table)
        {
            return new SgAttributeItemName(attributeSetId: table.AttributeSetId.Read(record), name: table.Name.Read(record));
        }

        public static SgAttributeItemName ReadOrdinal(ISqDataRecordReader record, TblAttributeSet table, int offset)
        {
            return new SgAttributeItemName(attributeSetId: table.AttributeSetId.Read(record, offset), name: table.Name.Read(record, offset + 1));
        }

        public Guid AttributeSetId { get; }

        public string Name { get; }

        public static TableColumn[] GetColumns(TblAttributeSet table)
        {
            return new TableColumn[]{table.AttributeSetId, table.Name};
        }

        public static IRecordSetterNext GetMapping(IDataMapSetter<TblAttributeSet, SgAttributeItemName> s)
        {
            return s.Set(s.Target.AttributeSetId, s.Source.AttributeSetId).Set(s.Target.Name, s.Source.Name);
        }

        public static IRecordSetterNext GetUpdateKeyMapping(IDataMapSetter<TblAttributeSet, SgAttributeItemName> s)
        {
            return s.Set(s.Target.AttributeSetId, s.Source.AttributeSetId);
        }

        public static IRecordSetterNext GetUpdateMapping(IDataMapSetter<TblAttributeSet, SgAttributeItemName> s)
        {
            return s.Set(s.Target.Name, s.Source.Name);
        }

        public static ISqModelReader<SgAttributeItemName, TblAttributeSet> GetReader()
        {
            return SgAttributeItemNameReader.Instance;
        }

        private class SgAttributeItemNameReader : ISqModelReader<SgAttributeItemName, TblAttributeSet>
        {
            public static SgAttributeItemNameReader Instance { get; } = new SgAttributeItemNameReader();
            IReadOnlyList<ExprColumn> ISqModelReader<SgAttributeItemName, TblAttributeSet>.GetColumns(TblAttributeSet table)
            {
                return SgAttributeItemName.GetColumns(table);
            }

            SgAttributeItemName ISqModelReader<SgAttributeItemName, TblAttributeSet>.Read(ISqDataRecordReader record, TblAttributeSet table)
            {
                return SgAttributeItemName.Read(record, table);
            }

            SgAttributeItemName ISqModelReader<SgAttributeItemName, TblAttributeSet>.ReadOrdinal(ISqDataRecordReader record, TblAttributeSet table, int offset)
            {
                return SgAttributeItemName.ReadOrdinal(record, table, offset);
            }
        }

        public static ISqModelUpdaterKey<SgAttributeItemName, TblAttributeSet> GetUpdater()
        {
            return SgAttributeItemNameUpdater.Instance;
        }

        private class SgAttributeItemNameUpdater : ISqModelUpdaterKey<SgAttributeItemName, TblAttributeSet>
        {
            public static SgAttributeItemNameUpdater Instance { get; } = new SgAttributeItemNameUpdater();
            IRecordSetterNext ISqModelUpdater<SgAttributeItemName, TblAttributeSet>.GetMapping(IDataMapSetter<TblAttributeSet, SgAttributeItemName> dataMapSetter)
            {
                return SgAttributeItemName.GetMapping(dataMapSetter);
            }

            IRecordSetterNext ISqModelUpdaterKey<SgAttributeItemName, TblAttributeSet>.GetUpdateKeyMapping(IDataMapSetter<TblAttributeSet, SgAttributeItemName> dataMapSetter)
            {
                return SgAttributeItemName.GetUpdateKeyMapping(dataMapSetter);
            }

            IRecordSetterNext ISqModelUpdaterKey<SgAttributeItemName, TblAttributeSet>.GetUpdateMapping(IDataMapSetter<TblAttributeSet, SgAttributeItemName> dataMapSetter)
            {
                return SgAttributeItemName.GetUpdateMapping(dataMapSetter);
            }
        }

        public Guid AttributeId { get; }
    }
}