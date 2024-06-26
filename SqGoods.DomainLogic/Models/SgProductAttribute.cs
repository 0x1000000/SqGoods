using System;
using SqExpress;
using SqExpress.QueryBuilders.RecordSetter;
using SqGoods.DomainLogic.Tables;
using SqExpress.Syntax.Names;
using System.Collections.Generic;
using SqExpress.Syntax.Select.SelectItems;

namespace SqGoods.DomainLogic.Models
{
    public record SgProductAttribute
    {
        //Auto-generated by SqExpress Code-gen util
        public SgProductAttribute(Guid productId, Guid attributeId, string? stringValue, int? intValue, bool? boolValue, Guid? guidValue)
        {
            this.ProductId = productId;
            this.AttributeId = attributeId;
            this.StringValue = stringValue;
            this.IntValue = intValue;
            this.BoolValue = boolValue;
            this.GuidValue = guidValue;
        }

        //Auto-generated by SqExpress Code-gen util
        public static SgProductAttribute Read(ISqDataRecordReader record, TblProductAttribute table)
        {
            return new SgProductAttribute(productId: table.ProductId.Read(record), attributeId: table.AttributeId.Read(record), stringValue: table.StringValue.Read(record), intValue: table.IntValue.Read(record), boolValue: table.BoolValue.Read(record), guidValue: table.GuidValue.Read(record));
        }

        //Auto-generated by SqExpress Code-gen util
        public static SgProductAttribute ReadWithPrefix(ISqDataRecordReader record, TblProductAttribute table, string prefix)
        {
            return new SgProductAttribute(productId: table.ProductId.Read(record, prefix + table.ProductId.ColumnName.Name), attributeId: table.AttributeId.Read(record, prefix + table.AttributeId.ColumnName.Name), stringValue: table.StringValue.Read(record, prefix + table.StringValue.ColumnName.Name), intValue: table.IntValue.Read(record, prefix + table.IntValue.ColumnName.Name), boolValue: table.BoolValue.Read(record, prefix + table.BoolValue.ColumnName.Name), guidValue: table.GuidValue.Read(record, prefix + table.GuidValue.ColumnName.Name));
        }

        //Auto-generated by SqExpress Code-gen util
        public static SgProductAttribute ReadOrdinal(ISqDataRecordReader record, TblProductAttribute table, int offset)
        {
            return new SgProductAttribute(productId: table.ProductId.Read(record, offset), attributeId: table.AttributeId.Read(record, offset + 1), stringValue: table.StringValue.Read(record, offset + 2), intValue: table.IntValue.Read(record, offset + 3), boolValue: table.BoolValue.Read(record, offset + 4), guidValue: table.GuidValue.Read(record, offset + 5));
        }

        //Auto-generated by SqExpress Code-gen util
        public Guid ProductId { get; }

        //Auto-generated by SqExpress Code-gen util
        public Guid AttributeId { get; }

        //Auto-generated by SqExpress Code-gen util
        public string? StringValue { get; }

        //Auto-generated by SqExpress Code-gen util
        public int? IntValue { get; }

        //Auto-generated by SqExpress Code-gen util
        public bool? BoolValue { get; }

        //Auto-generated by SqExpress Code-gen util
        public Guid? GuidValue { get; }

        //Auto-generated by SqExpress Code-gen util
        public static TableColumn[] GetColumns(TblProductAttribute table)
        {
            return new TableColumn[]{table.ProductId, table.AttributeId, table.StringValue, table.IntValue, table.BoolValue, table.GuidValue};
        }

        //Auto-generated by SqExpress Code-gen util
        public static ExprAliasedColumn[] GetColumnsWithPrefix(TblProductAttribute table, string prefix)
        {
            return new ExprAliasedColumn[]{table.ProductId.As(prefix + table.ProductId.ColumnName.Name), table.AttributeId.As(prefix + table.AttributeId.ColumnName.Name), table.StringValue.As(prefix + table.StringValue.ColumnName.Name), table.IntValue.As(prefix + table.IntValue.ColumnName.Name), table.BoolValue.As(prefix + table.BoolValue.ColumnName.Name), table.GuidValue.As(prefix + table.GuidValue.ColumnName.Name)};
        }

        //Auto-generated by SqExpress Code-gen util
        public static bool IsNull(ISqDataRecordReader record, TblProductAttribute table)
        {
            foreach (var column in GetColumns(table))
            {
                if (!record.IsDBNull(column.ColumnName.Name))
                {
                    return false;
                }
            }

            return true;
        }

        //Auto-generated by SqExpress Code-gen util
        public static bool IsNullWithPrefix(ISqDataRecordReader record, TblProductAttribute table, string prefix)
        {
            foreach (var column in GetColumnsWithPrefix(table, prefix))
            {
                if (!record.IsDBNull(column.Alias!.Name))
                {
                    return false;
                }
            }

            return true;
        }

        //Auto-generated by SqExpress Code-gen util
        public static IRecordSetterNext GetMapping(IDataMapSetter<TblProductAttribute, SgProductAttribute> s)
        {
            return s.Set(s.Target.ProductId, s.Source.ProductId).Set(s.Target.AttributeId, s.Source.AttributeId).Set(s.Target.StringValue, s.Source.StringValue).Set(s.Target.IntValue, s.Source.IntValue).Set(s.Target.BoolValue, s.Source.BoolValue).Set(s.Target.GuidValue, s.Source.GuidValue);
        }

        //Auto-generated by SqExpress Code-gen util
        public static IRecordSetterNext GetUpdateKeyMapping(IDataMapSetter<TblProductAttribute, SgProductAttribute> s)
        {
            return s.Set(s.Target.ProductId, s.Source.ProductId).Set(s.Target.AttributeId, s.Source.AttributeId);
        }

        //Auto-generated by SqExpress Code-gen util
        public static IRecordSetterNext GetUpdateMapping(IDataMapSetter<TblProductAttribute, SgProductAttribute> s)
        {
            return s.Set(s.Target.StringValue, s.Source.StringValue).Set(s.Target.IntValue, s.Source.IntValue).Set(s.Target.BoolValue, s.Source.BoolValue).Set(s.Target.GuidValue, s.Source.GuidValue);
        }

        //Auto-generated by SqExpress Code-gen util
        public static ISqModelReader<SgProductAttribute, TblProductAttribute> GetReader()
        {
            return SgProductAttributeReader.Instance;
        }

        //Auto-generated by SqExpress Code-gen util
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

        //Auto-generated by SqExpress Code-gen util
        public static ISqModelUpdaterKey<SgProductAttribute, TblProductAttribute> GetUpdater()
        {
            return SgProductAttributeUpdater.Instance;
        }

        //Auto-generated by SqExpress Code-gen util
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