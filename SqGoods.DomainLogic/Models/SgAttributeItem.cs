using System;
using SqExpress;
using SqExpress.QueryBuilders.RecordSetter;
using SqGoods.DomainLogic.Tables;
using SqExpress.Syntax.Names;
using System.Collections.Generic;

namespace SqGoods.DomainLogic.Models
{
    public record SgAttributeItem
    {
        public SgAttributeItem(Guid attributeSetId, Guid attributeId, string name, int order)
        {
            this.AttributeSetId = attributeSetId;
            this.AttributeId = attributeId;
            this.Name = name;
            this.Order = order;
        }

        public static SgAttributeItem Read(ISqDataRecordReader record, TblAttributeSet table)
        {
            return new SgAttributeItem(attributeSetId: table.AttributeSetId.Read(record), attributeId: table.AttributeId.Read(record), name: table.Name.Read(record), order: table.Order.Read(record));
        }

        public static SgAttributeItem ReadOrdinal(ISqDataRecordReader record, TblAttributeSet table, int offset)
        {
            return new SgAttributeItem(attributeSetId: table.AttributeSetId.Read(record, offset), attributeId: table.AttributeId.Read(record, offset + 1), name: table.Name.Read(record, offset + 2), order: table.Order.Read(record, offset + 3));
        }

        public Guid AttributeSetId { get; }

        public Guid AttributeId { get; }

        public string Name { get; }

        public int Order { get; }

        public static TableColumn[] GetColumns(TblAttributeSet table)
        {
            return new TableColumn[]{table.AttributeSetId, table.AttributeId, table.Name, table.Order};
        }

        public static IRecordSetterNext GetMapping(IDataMapSetter<TblAttributeSet, SgAttributeItem> s)
        {
            return s.Set(s.Target.AttributeSetId, s.Source.AttributeSetId).Set(s.Target.AttributeId, s.Source.AttributeId).Set(s.Target.Name, s.Source.Name).Set(s.Target.Order, s.Source.Order);
        }

        public static IRecordSetterNext GetUpdateKeyMapping(IDataMapSetter<TblAttributeSet, SgAttributeItem> s)
        {
            return s.Set(s.Target.AttributeSetId, s.Source.AttributeSetId);
        }

        public static IRecordSetterNext GetUpdateMapping(IDataMapSetter<TblAttributeSet, SgAttributeItem> s)
        {
            return s.Set(s.Target.AttributeId, s.Source.AttributeId).Set(s.Target.Name, s.Source.Name).Set(s.Target.Order, s.Source.Order);
        }

        public static ISqModelReader<SgAttributeItem, TblAttributeSet> GetReader()
        {
            return SgAttributeItemReader.Instance;
        }

        private class SgAttributeItemReader : ISqModelReader<SgAttributeItem, TblAttributeSet>
        {
            public static SgAttributeItemReader Instance { get; } = new SgAttributeItemReader();
            IReadOnlyList<ExprColumn> ISqModelReader<SgAttributeItem, TblAttributeSet>.GetColumns(TblAttributeSet table)
            {
                return SgAttributeItem.GetColumns(table);
            }

            SgAttributeItem ISqModelReader<SgAttributeItem, TblAttributeSet>.Read(ISqDataRecordReader record, TblAttributeSet table)
            {
                return SgAttributeItem.Read(record, table);
            }

            SgAttributeItem ISqModelReader<SgAttributeItem, TblAttributeSet>.ReadOrdinal(ISqDataRecordReader record, TblAttributeSet table, int offset)
            {
                return SgAttributeItem.ReadOrdinal(record, table, offset);
            }
        }

        public static ISqModelUpdaterKey<SgAttributeItem, TblAttributeSet> GetUpdater()
        {
            return SgAttributeItemUpdater.Instance;
        }

        private class SgAttributeItemUpdater : ISqModelUpdaterKey<SgAttributeItem, TblAttributeSet>
        {
            public static SgAttributeItemUpdater Instance { get; } = new SgAttributeItemUpdater();
            IRecordSetterNext ISqModelUpdater<SgAttributeItem, TblAttributeSet>.GetMapping(IDataMapSetter<TblAttributeSet, SgAttributeItem> dataMapSetter)
            {
                return SgAttributeItem.GetMapping(dataMapSetter);
            }

            IRecordSetterNext ISqModelUpdaterKey<SgAttributeItem, TblAttributeSet>.GetUpdateKeyMapping(IDataMapSetter<TblAttributeSet, SgAttributeItem> dataMapSetter)
            {
                return SgAttributeItem.GetUpdateKeyMapping(dataMapSetter);
            }

            IRecordSetterNext ISqModelUpdaterKey<SgAttributeItem, TblAttributeSet>.GetUpdateMapping(IDataMapSetter<TblAttributeSet, SgAttributeItem> dataMapSetter)
            {
                return SgAttributeItem.GetUpdateMapping(dataMapSetter);
            }
        }
    }
}