using System;
using SqExpress;
using SqExpress.QueryBuilders.RecordSetter;
using SqGoods.DomainLogic.Tables;
using SqExpress.Syntax.Names;
using System.Collections.Generic;

namespace SqGoods.DomainLogic.Models
{
    public record SgProductAttribute
    {
        public SgProductAttribute(Guid productId, Guid attributeId, string? stringValue, int? intValue, bool? boolValue, Guid? selectValue)
        {
            this.ProductId = productId;
            this.AttributeId = attributeId;
            this.StringValue = stringValue;
            this.IntValue = intValue;
            this.BoolValue = boolValue;
            this.SelectValue = selectValue;
        }

        public static SgProductAttribute Read(ISqDataRecordReader record, TblProductAttribute table)
        {
            return new SgProductAttribute(productId: table.ProductId.Read(record), attributeId: table.AttributeId.Read(record), stringValue: table.StringValue.Read(record), intValue: table.IntValue.Read(record), boolValue: table.BoolValue.Read(record), selectValue: table.SelectValue.Read(record));
        }

        public static SgProductAttribute ReadOrdinal(ISqDataRecordReader record, TblProductAttribute table, int offset)
        {
            return new SgProductAttribute(productId: table.ProductId.Read(record, offset), attributeId: table.AttributeId.Read(record, offset + 1), stringValue: table.StringValue.Read(record, offset + 2), intValue: table.IntValue.Read(record, offset + 3), boolValue: table.BoolValue.Read(record, offset + 4), selectValue: table.SelectValue.Read(record, offset + 5));
        }

        public Guid ProductId { get; }

        public Guid AttributeId { get; }

        public string? StringValue { get; }

        public int? IntValue { get; }

        public bool? BoolValue { get; }

        public Guid? SelectValue { get; }

        public static TableColumn[] GetColumns(TblProductAttribute table)
        {
            return new TableColumn[]{table.ProductId, table.AttributeId, table.StringValue, table.IntValue, table.BoolValue, table.SelectValue};
        }

        public static IRecordSetterNext GetMapping(IDataMapSetter<TblProductAttribute, SgProductAttribute> s)
        {
            return s.Set(s.Target.ProductId, s.Source.ProductId).Set(s.Target.AttributeId, s.Source.AttributeId).Set(s.Target.StringValue, s.Source.StringValue).Set(s.Target.IntValue, s.Source.IntValue).Set(s.Target.BoolValue, s.Source.BoolValue).Set(s.Target.SelectValue, s.Source.SelectValue);
        }

        public static IRecordSetterNext GetUpdateKeyMapping(IDataMapSetter<TblProductAttribute, SgProductAttribute> s)
        {
            return s.Set(s.Target.ProductId, s.Source.ProductId).Set(s.Target.AttributeId, s.Source.AttributeId);
        }

        public static IRecordSetterNext GetUpdateMapping(IDataMapSetter<TblProductAttribute, SgProductAttribute> s)
        {
            return s.Set(s.Target.StringValue, s.Source.StringValue).Set(s.Target.IntValue, s.Source.IntValue).Set(s.Target.BoolValue, s.Source.BoolValue).Set(s.Target.SelectValue, s.Source.SelectValue);
        }

        public static ISqModelReader<SgProductAttribute, TblProductAttribute> GetReader()
        {
            return SgProductAttributeReader.Instance;
        }

        private class SgProductAttributeReader : ISqModelReader<SgProductAttribute, TblProductAttribute>
        {
            public static SgProductAttributeReader Instance { get; } = new SgProductAttributeReader();
            IReadOnlyList<ExprColumn> ISqModelReader<SgProductAttribute, TblProductAttribute>.GetColumns(TblProductAttribute table)
            {
                return SgProductAttribute.GetColumns(table);
            }

            SgProductAttribute ISqModelReader<SgProductAttribute, TblProductAttribute>.Read(ISqDataRecordReader record, TblProductAttribute table)
            {
                return SgProductAttribute.Read(record, table);
            }

            SgProductAttribute ISqModelReader<SgProductAttribute, TblProductAttribute>.ReadOrdinal(ISqDataRecordReader record, TblProductAttribute table, int offset)
            {
                return SgProductAttribute.ReadOrdinal(record, table, offset);
            }
        }

        public static ISqModelUpdaterKey<SgProductAttribute, TblProductAttribute> GetUpdater()
        {
            return SgProductAttributeUpdater.Instance;
        }

        private class SgProductAttributeUpdater : ISqModelUpdaterKey<SgProductAttribute, TblProductAttribute>
        {
            public static SgProductAttributeUpdater Instance { get; } = new SgProductAttributeUpdater();
            IRecordSetterNext ISqModelUpdater<SgProductAttribute, TblProductAttribute>.GetMapping(IDataMapSetter<TblProductAttribute, SgProductAttribute> dataMapSetter)
            {
                return SgProductAttribute.GetMapping(dataMapSetter);
            }

            IRecordSetterNext ISqModelUpdaterKey<SgProductAttribute, TblProductAttribute>.GetUpdateKeyMapping(IDataMapSetter<TblProductAttribute, SgProductAttribute> dataMapSetter)
            {
                return SgProductAttribute.GetUpdateKeyMapping(dataMapSetter);
            }

            IRecordSetterNext ISqModelUpdaterKey<SgProductAttribute, TblProductAttribute>.GetUpdateMapping(IDataMapSetter<TblProductAttribute, SgProductAttribute> dataMapSetter)
            {
                return SgProductAttribute.GetUpdateMapping(dataMapSetter);
            }
        }
    }
}