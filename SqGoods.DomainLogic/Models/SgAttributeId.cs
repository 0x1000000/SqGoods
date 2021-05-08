using System;
using SqExpress;
using SqExpress.QueryBuilders.RecordSetter;
using SqGoods.DomainLogic.Tables;
using SqExpress.Syntax.Names;
using System.Collections.Generic;

namespace SqGoods.DomainLogic.Models
{
    public record SgAttributeId
    {
        public SgAttributeId(Guid id)
        {
            this.Id = id;
        }

        public static SgAttributeId Read(ISqDataRecordReader record, TblAttribute table)
        {
            return new SgAttributeId(id: table.AttributeId.Read(record));
        }

        public static SgAttributeId ReadOrdinal(ISqDataRecordReader record, TblAttribute table, int offset)
        {
            return new SgAttributeId(id: table.AttributeId.Read(record, offset));
        }

        public Guid Id { get; }

        public static TableColumn[] GetColumns(TblAttribute table)
        {
            return new TableColumn[]{table.AttributeId};
        }

        public static IRecordSetterNext GetMapping(IDataMapSetter<TblAttribute, SgAttributeId> s)
        {
            return s.Set(s.Target.AttributeId, s.Source.Id);
        }

        public static ISqModelReader<SgAttributeId, TblAttribute> GetReader()
        {
            return SgAttributeIdReader.Instance;
        }

        private class SgAttributeIdReader : ISqModelReader<SgAttributeId, TblAttribute>
        {
            public static SgAttributeIdReader Instance { get; } = new SgAttributeIdReader();
            IReadOnlyList<ExprColumn> ISqModelReader<SgAttributeId, TblAttribute>.GetColumns(TblAttribute table)
            {
                return SgAttributeId.GetColumns(table);
            }

            SgAttributeId ISqModelReader<SgAttributeId, TblAttribute>.Read(ISqDataRecordReader record, TblAttribute table)
            {
                return SgAttributeId.Read(record, table);
            }

            SgAttributeId ISqModelReader<SgAttributeId, TblAttribute>.ReadOrdinal(ISqDataRecordReader record, TblAttribute table, int offset)
            {
                return SgAttributeId.ReadOrdinal(record, table, offset);
            }
        }

        public static ISqModelUpdater<SgAttributeId, TblAttribute> GetUpdater()
        {
            return SgAttributeIdUpdater.Instance;
        }

        private class SgAttributeIdUpdater : ISqModelUpdater<SgAttributeId, TblAttribute>
        {
            public static SgAttributeIdUpdater Instance { get; } = new SgAttributeIdUpdater();
            IRecordSetterNext ISqModelUpdater<SgAttributeId, TblAttribute>.GetMapping(IDataMapSetter<TblAttribute, SgAttributeId> dataMapSetter)
            {
                return SgAttributeId.GetMapping(dataMapSetter);
            }
        }
    }
}