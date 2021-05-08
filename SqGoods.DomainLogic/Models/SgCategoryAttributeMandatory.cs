using System;
using SqExpress;
using SqExpress.QueryBuilders.RecordSetter;
using SqGoods.DomainLogic.Tables;
using SqExpress.Syntax.Names;
using System.Collections.Generic;

namespace SqGoods.DomainLogic.Models
{
    public record SgCategoryAttributeMandatory
    {
        public SgCategoryAttributeMandatory(Guid categoryId, Guid attributeId, bool mandatory)
        {
            this.CategoryId = categoryId;
            this.AttributeId = attributeId;
            this.Mandatory = mandatory;
        }

        public static SgCategoryAttributeMandatory Read(ISqDataRecordReader record, TblCategoryAttribute table)
        {
            return new SgCategoryAttributeMandatory(categoryId: table.CategoryId.Read(record), attributeId: table.AttributeId.Read(record), mandatory: table.Mandatory.Read(record));
        }

        public static SgCategoryAttributeMandatory ReadOrdinal(ISqDataRecordReader record, TblCategoryAttribute table, int offset)
        {
            return new SgCategoryAttributeMandatory(categoryId: table.CategoryId.Read(record, offset), attributeId: table.AttributeId.Read(record, offset + 1), mandatory: table.Mandatory.Read(record, offset + 2));
        }

        public Guid CategoryId { get; }

        public Guid AttributeId { get; }

        public bool Mandatory { get; }

        public static TableColumn[] GetColumns(TblCategoryAttribute table)
        {
            return new TableColumn[]{table.CategoryId, table.AttributeId, table.Mandatory};
        }

        public static IRecordSetterNext GetMapping(IDataMapSetter<TblCategoryAttribute, SgCategoryAttributeMandatory> s)
        {
            return s.Set(s.Target.CategoryId, s.Source.CategoryId).Set(s.Target.AttributeId, s.Source.AttributeId).Set(s.Target.Mandatory, s.Source.Mandatory);
        }

        public static IRecordSetterNext GetUpdateKeyMapping(IDataMapSetter<TblCategoryAttribute, SgCategoryAttributeMandatory> s)
        {
            return s.Set(s.Target.CategoryId, s.Source.CategoryId).Set(s.Target.AttributeId, s.Source.AttributeId);
        }

        public static IRecordSetterNext GetUpdateMapping(IDataMapSetter<TblCategoryAttribute, SgCategoryAttributeMandatory> s)
        {
            return s.Set(s.Target.Mandatory, s.Source.Mandatory);
        }

        public static ISqModelReader<SgCategoryAttributeMandatory, TblCategoryAttribute> GetReader()
        {
            return SgCategoryAttributeMandatoryReader.Instance;
        }

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

        public static ISqModelUpdaterKey<SgCategoryAttributeMandatory, TblCategoryAttribute> GetUpdater()
        {
            return SgCategoryAttributeMandatoryUpdater.Instance;
        }

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