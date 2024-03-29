using System;
using SqExpress;
using SqExpress.QueryBuilders.RecordSetter;
using SqGoods.DomainLogic.Tables;
using SqExpress.Syntax.Names;
using System.Collections.Generic;
using SqExpress.Syntax.Select.SelectItems;

namespace SqGoods.DomainLogic.Models
{
    public record SgCategoryAttributeMandatory
    {
        //Auto-generated by SqExpress Code-gen util
        public SgCategoryAttributeMandatory(Guid categoryId, Guid attributeId, bool mandatory)
        {
            this.CategoryId = categoryId;
            this.AttributeId = attributeId;
            this.Mandatory = mandatory;
        }

        //Auto-generated by SqExpress Code-gen util
        public static SgCategoryAttributeMandatory Read(ISqDataRecordReader record, TblCategoryAttribute table)
        {
            return new SgCategoryAttributeMandatory(categoryId: table.CategoryId.Read(record), attributeId: table.AttributeId.Read(record), mandatory: table.Mandatory.Read(record));
        }

        //Auto-generated by SqExpress Code-gen util
        public static SgCategoryAttributeMandatory ReadWithPrefix(ISqDataRecordReader record, TblCategoryAttribute table, string prefix)
        {
            return new SgCategoryAttributeMandatory(categoryId: table.CategoryId.Read(record, prefix + table.CategoryId.ColumnName.Name), attributeId: table.AttributeId.Read(record, prefix + table.AttributeId.ColumnName.Name), mandatory: table.Mandatory.Read(record, prefix + table.Mandatory.ColumnName.Name));
        }

        //Auto-generated by SqExpress Code-gen util
        public static SgCategoryAttributeMandatory ReadOrdinal(ISqDataRecordReader record, TblCategoryAttribute table, int offset)
        {
            return new SgCategoryAttributeMandatory(categoryId: table.CategoryId.Read(record, offset), attributeId: table.AttributeId.Read(record, offset + 1), mandatory: table.Mandatory.Read(record, offset + 2));
        }

        //Auto-generated by SqExpress Code-gen util
        public Guid CategoryId { get; }

        //Auto-generated by SqExpress Code-gen util
        public Guid AttributeId { get; }

        //Auto-generated by SqExpress Code-gen util
        public bool Mandatory { get; }

        //Auto-generated by SqExpress Code-gen util
        public static TableColumn[] GetColumns(TblCategoryAttribute table)
        {
            return new TableColumn[]{table.CategoryId, table.AttributeId, table.Mandatory};
        }

        //Auto-generated by SqExpress Code-gen util
        public static ExprAliasedColumn[] GetColumnsWithPrefix(TblCategoryAttribute table, string prefix)
        {
            return new ExprAliasedColumn[]{table.CategoryId.As(prefix + table.CategoryId.ColumnName.Name), table.AttributeId.As(prefix + table.AttributeId.ColumnName.Name), table.Mandatory.As(prefix + table.Mandatory.ColumnName.Name)};
        }

        //Auto-generated by SqExpress Code-gen util
        public static bool IsNull(ISqDataRecordReader record, TblCategoryAttribute table)
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
        public static bool IsNullWithPrefix(ISqDataRecordReader record, TblCategoryAttribute table, string prefix)
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
        public static IRecordSetterNext GetMapping(IDataMapSetter<TblCategoryAttribute, SgCategoryAttributeMandatory> s)
        {
            return s.Set(s.Target.CategoryId, s.Source.CategoryId).Set(s.Target.AttributeId, s.Source.AttributeId).Set(s.Target.Mandatory, s.Source.Mandatory);
        }

        //Auto-generated by SqExpress Code-gen util
        public static IRecordSetterNext GetUpdateKeyMapping(IDataMapSetter<TblCategoryAttribute, SgCategoryAttributeMandatory> s)
        {
            return s.Set(s.Target.CategoryId, s.Source.CategoryId).Set(s.Target.AttributeId, s.Source.AttributeId);
        }

        //Auto-generated by SqExpress Code-gen util
        public static IRecordSetterNext GetUpdateMapping(IDataMapSetter<TblCategoryAttribute, SgCategoryAttributeMandatory> s)
        {
            return s.Set(s.Target.Mandatory, s.Source.Mandatory);
        }

        //Auto-generated by SqExpress Code-gen util
        public static ISqModelReader<SgCategoryAttributeMandatory, TblCategoryAttribute> GetReader()
        {
            return SgCategoryAttributeMandatoryReader.Instance;
        }

        //Auto-generated by SqExpress Code-gen util
        private class SgCategoryAttributeMandatoryReader : ISqModelReader<SgCategoryAttributeMandatory, TblCategoryAttribute>
        {
            public static SgCategoryAttributeMandatoryReader Instance { get; } = new SgCategoryAttributeMandatoryReader();
            IReadOnlyList<ExprColumn> ISqModelReader<SgCategoryAttributeMandatory, TblCategoryAttribute>.GetColumns(TblCategoryAttribute table)
            {
                return SgCategoryAttributeMandatory.GetColumns(table);
            }

            SgCategoryAttributeMandatory ISqModelReader<SgCategoryAttributeMandatory, TblCategoryAttribute>.Read(ISqDataRecordReader record, TblCategoryAttribute table)
            {
                return SgCategoryAttributeMandatory.Read(record, table);
            }

            SgCategoryAttributeMandatory ISqModelReader<SgCategoryAttributeMandatory, TblCategoryAttribute>.ReadOrdinal(ISqDataRecordReader record, TblCategoryAttribute table, int offset)
            {
                return SgCategoryAttributeMandatory.ReadOrdinal(record, table, offset);
            }
        }

        //Auto-generated by SqExpress Code-gen util
        public static ISqModelUpdaterKey<SgCategoryAttributeMandatory, TblCategoryAttribute> GetUpdater()
        {
            return SgCategoryAttributeMandatoryUpdater.Instance;
        }

        //Auto-generated by SqExpress Code-gen util
        private class SgCategoryAttributeMandatoryUpdater : ISqModelUpdaterKey<SgCategoryAttributeMandatory, TblCategoryAttribute>
        {
            public static SgCategoryAttributeMandatoryUpdater Instance { get; } = new SgCategoryAttributeMandatoryUpdater();
            IRecordSetterNext ISqModelUpdater<SgCategoryAttributeMandatory, TblCategoryAttribute>.GetMapping(IDataMapSetter<TblCategoryAttribute, SgCategoryAttributeMandatory> dataMapSetter)
            {
                return SgCategoryAttributeMandatory.GetMapping(dataMapSetter);
            }

            IRecordSetterNext ISqModelUpdaterKey<SgCategoryAttributeMandatory, TblCategoryAttribute>.GetUpdateKeyMapping(IDataMapSetter<TblCategoryAttribute, SgCategoryAttributeMandatory> dataMapSetter)
            {
                return SgCategoryAttributeMandatory.GetUpdateKeyMapping(dataMapSetter);
            }

            IRecordSetterNext ISqModelUpdaterKey<SgCategoryAttributeMandatory, TblCategoryAttribute>.GetUpdateMapping(IDataMapSetter<TblCategoryAttribute, SgCategoryAttributeMandatory> dataMapSetter)
            {
                return SgCategoryAttributeMandatory.GetUpdateMapping(dataMapSetter);
            }
        }

        private class SgCategoryAttributeTupleReader : ISqModelReader<SgCategoryAttributeMandatory, TblCategoryAttribute>
        {
            public static SgCategoryAttributeTupleReader Instance { get; } = new SgCategoryAttributeTupleReader();
            IReadOnlyList<ExprColumn> ISqModelReader<SgCategoryAttributeMandatory, TblCategoryAttribute>.GetColumns(TblCategoryAttribute table)
            {
                return SgCategoryAttributeMandatory.GetColumns(table);
            }

            SgCategoryAttributeMandatory ISqModelReader<SgCategoryAttributeMandatory, TblCategoryAttribute>.Read(ISqDataRecordReader record, TblCategoryAttribute table)
            {
                return SgCategoryAttributeMandatory.Read(record, table);
            }

            SgCategoryAttributeMandatory ISqModelReader<SgCategoryAttributeMandatory, TblCategoryAttribute>.ReadOrdinal(ISqDataRecordReader record, TblCategoryAttribute table, int offset)
            {
                return SgCategoryAttributeMandatory.ReadOrdinal(record, table, offset);
            }
        }

        private class SgCategoryAttributeTupleUpdater : ISqModelUpdaterKey<SgCategoryAttributeMandatory, TblCategoryAttribute>
        {
            public static SgCategoryAttributeTupleUpdater Instance { get; } = new SgCategoryAttributeTupleUpdater();
            IRecordSetterNext ISqModelUpdater<SgCategoryAttributeMandatory, TblCategoryAttribute>.GetMapping(IDataMapSetter<TblCategoryAttribute, SgCategoryAttributeMandatory> dataMapSetter)
            {
                return SgCategoryAttributeMandatory.GetMapping(dataMapSetter);
            }

            IRecordSetterNext ISqModelUpdaterKey<SgCategoryAttributeMandatory, TblCategoryAttribute>.GetUpdateKeyMapping(IDataMapSetter<TblCategoryAttribute, SgCategoryAttributeMandatory> dataMapSetter)
            {
                return SgCategoryAttributeMandatory.GetUpdateKeyMapping(dataMapSetter);
            }

            IRecordSetterNext ISqModelUpdaterKey<SgCategoryAttributeMandatory, TblCategoryAttribute>.GetUpdateMapping(IDataMapSetter<TblCategoryAttribute, SgCategoryAttributeMandatory> dataMapSetter)
            {
                return SgCategoryAttributeMandatory.GetUpdateMapping(dataMapSetter);
            }
        }
    }
}