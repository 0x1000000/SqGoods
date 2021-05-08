using System;
using SqExpress;
using SqExpress.QueryBuilders.RecordSetter;
using SqGoods.DomainLogic.Tables;
using SqExpress.Syntax.Names;
using System.Collections.Generic;

namespace SqGoods.DomainLogic.Models
{
    public record SgCategoryAttributeOrder
    {
        public SgCategoryAttributeOrder(Guid categoryId, Guid attributeId, int order)
        {
            this.CategoryId = categoryId;
            this.AttributeId = attributeId;
            this.Order = order;
        }

        public static SgCategoryAttributeOrder Read(ISqDataRecordReader record, TblCategoryAttribute table)
        {
            return new SgCategoryAttributeOrder(categoryId: table.CategoryId.Read(record), attributeId: table.AttributeId.Read(record), order: table.Order.Read(record));
        }

        public static SgCategoryAttributeOrder ReadOrdinal(ISqDataRecordReader record, TblCategoryAttribute table, int offset)
        {
            return new SgCategoryAttributeOrder(categoryId: table.CategoryId.Read(record, offset), attributeId: table.AttributeId.Read(record, offset + 1), order: table.Order.Read(record, offset + 2));
        }

        public Guid CategoryId { get; }

        public Guid AttributeId { get; }

        public int Order { get; }

        public static TableColumn[] GetColumns(TblCategoryAttribute table)
        {
            return new TableColumn[]{table.CategoryId, table.AttributeId, table.Order};
        }

        public static IRecordSetterNext GetMapping(IDataMapSetter<TblCategoryAttribute, SgCategoryAttributeOrder> s)
        {
            return s.Set(s.Target.CategoryId, s.Source.CategoryId).Set(s.Target.AttributeId, s.Source.AttributeId).Set(s.Target.Order, s.Source.Order);
        }

        public static IRecordSetterNext GetUpdateKeyMapping(IDataMapSetter<TblCategoryAttribute, SgCategoryAttributeOrder> s)
        {
            return s.Set(s.Target.CategoryId, s.Source.CategoryId).Set(s.Target.AttributeId, s.Source.AttributeId);
        }

        public static IRecordSetterNext GetUpdateMapping(IDataMapSetter<TblCategoryAttribute, SgCategoryAttributeOrder> s)
        {
            return s.Set(s.Target.Order, s.Source.Order);
        }

        public static ISqModelReader<SgCategoryAttributeOrder, TblCategoryAttribute> GetReader()
        {
            return SgCategoryAttributeOrderReader.Instance;
        }

        private class SgCategoryAttributeOrderReader : ISqModelReader<SgCategoryAttributeOrder, TblCategoryAttribute>
        {
            public static SgCategoryAttributeOrderReader Instance { get; } = new SgCategoryAttributeOrderReader();
            IReadOnlyList<ExprColumn> ISqModelReader<SgCategoryAttributeOrder, TblCategoryAttribute>.GetColumns(TblCategoryAttribute table)
            {
                return SgCategoryAttributeOrder.GetColumns(table);
            }

            SgCategoryAttributeOrder ISqModelReader<SgCategoryAttributeOrder, TblCategoryAttribute>.Read(ISqDataRecordReader record, TblCategoryAttribute table)
            {
                return SgCategoryAttributeOrder.Read(record, table);
            }

            SgCategoryAttributeOrder ISqModelReader<SgCategoryAttributeOrder, TblCategoryAttribute>.ReadOrdinal(ISqDataRecordReader record, TblCategoryAttribute table, int offset)
            {
                return SgCategoryAttributeOrder.ReadOrdinal(record, table, offset);
            }
        }

        public static ISqModelUpdaterKey<SgCategoryAttributeOrder, TblCategoryAttribute> GetUpdater()
        {
            return SgCategoryAttributeOrderUpdater.Instance;
        }

        private class SgCategoryAttributeOrderUpdater : ISqModelUpdaterKey<SgCategoryAttributeOrder, TblCategoryAttribute>
        {
            public static SgCategoryAttributeOrderUpdater Instance { get; } = new SgCategoryAttributeOrderUpdater();
            IRecordSetterNext ISqModelUpdater<SgCategoryAttributeOrder, TblCategoryAttribute>.GetMapping(IDataMapSetter<TblCategoryAttribute, SgCategoryAttributeOrder> dataMapSetter)
            {
                return SgCategoryAttributeOrder.GetMapping(dataMapSetter);
            }

            IRecordSetterNext ISqModelUpdaterKey<SgCategoryAttributeOrder, TblCategoryAttribute>.GetUpdateKeyMapping(IDataMapSetter<TblCategoryAttribute, SgCategoryAttributeOrder> dataMapSetter)
            {
                return SgCategoryAttributeOrder.GetUpdateKeyMapping(dataMapSetter);
            }

            IRecordSetterNext ISqModelUpdaterKey<SgCategoryAttributeOrder, TblCategoryAttribute>.GetUpdateMapping(IDataMapSetter<TblCategoryAttribute, SgCategoryAttributeOrder> dataMapSetter)
            {
                return SgCategoryAttributeOrder.GetUpdateMapping(dataMapSetter);
            }
        }

        private class SgCategoryAttributeReader : ISqModelReader<SgCategoryAttributeOrder, TblCategoryAttribute>
        {
            public static SgCategoryAttributeReader Instance { get; } = new SgCategoryAttributeReader();
            IReadOnlyList<ExprColumn> ISqModelReader<SgCategoryAttributeOrder, TblCategoryAttribute>.GetColumns(TblCategoryAttribute table)
            {
                return SgCategoryAttributeOrder.GetColumns(table);
            }

            SgCategoryAttributeOrder ISqModelReader<SgCategoryAttributeOrder, TblCategoryAttribute>.Read(ISqDataRecordReader record, TblCategoryAttribute table)
            {
                return SgCategoryAttributeOrder.Read(record, table);
            }

            SgCategoryAttributeOrder ISqModelReader<SgCategoryAttributeOrder, TblCategoryAttribute>.ReadOrdinal(ISqDataRecordReader record, TblCategoryAttribute table, int offset)
            {
                return SgCategoryAttributeOrder.ReadOrdinal(record, table, offset);
            }
        }

        private class SgCategoryAttributeUpdater : ISqModelUpdaterKey<SgCategoryAttributeOrder, TblCategoryAttribute>
        {
            public static SgCategoryAttributeUpdater Instance { get; } = new SgCategoryAttributeUpdater();
            IRecordSetterNext ISqModelUpdater<SgCategoryAttributeOrder, TblCategoryAttribute>.GetMapping(IDataMapSetter<TblCategoryAttribute, SgCategoryAttributeOrder> dataMapSetter)
            {
                return SgCategoryAttributeOrder.GetMapping(dataMapSetter);
            }

            IRecordSetterNext ISqModelUpdaterKey<SgCategoryAttributeOrder, TblCategoryAttribute>.GetUpdateKeyMapping(IDataMapSetter<TblCategoryAttribute, SgCategoryAttributeOrder> dataMapSetter)
            {
                return SgCategoryAttributeOrder.GetUpdateKeyMapping(dataMapSetter);
            }

            IRecordSetterNext ISqModelUpdaterKey<SgCategoryAttributeOrder, TblCategoryAttribute>.GetUpdateMapping(IDataMapSetter<TblCategoryAttribute, SgCategoryAttributeOrder> dataMapSetter)
            {
                return SgCategoryAttributeOrder.GetUpdateMapping(dataMapSetter);
            }
        }
    }
}