using System;
using SqExpress;
using SqExpress.QueryBuilders.RecordSetter;
using SqGoods.DomainLogic.Tables;
using SqExpress.Syntax.Names;
using System.Collections.Generic;

namespace SqGoods.DomainLogic.Models
{
    public record SgAttributeTypeId
    {
        public SgAttributeTypeId(Guid id, SgAttributeType type)
        {
            this.Id = id;
            this.Type = type;
        }

        public static SgAttributeTypeId Read(ISqDataRecordReader record, TblAttribute table)
        {
            return new SgAttributeTypeId(id: table.AttributeId.Read(record), type: (SgAttributeType)table.Type.Read(record));
        }

        public static SgAttributeTypeId ReadOrdinal(ISqDataRecordReader record, TblAttribute table, int offset)
        {
            return new SgAttributeTypeId(id: table.AttributeId.Read(record, offset), type: (SgAttributeType)table.Type.Read(record, offset + 1));
        }

        public Guid Id { get; }

        public SgAttributeType Type { get; }

        public static TableColumn[] GetColumns(TblAttribute table)
        {
            return new TableColumn[]{table.AttributeId, table.Type};
        }

        public static IRecordSetterNext GetMapping(IDataMapSetter<TblAttribute, SgAttributeTypeId> s)
        {
            return s.Set(s.Target.AttributeId, s.Source.Id).Set(s.Target.Type, (short)s.Source.Type);
        }

        public static IRecordSetterNext GetUpdateKeyMapping(IDataMapSetter<TblAttribute, SgAttributeTypeId> s)
        {
            return s.Set(s.Target.AttributeId, s.Source.Id);
        }

        public static IRecordSetterNext GetUpdateMapping(IDataMapSetter<TblAttribute, SgAttributeTypeId> s)
        {
            return s.Set(s.Target.Type, (short)s.Source.Type);
        }

        public static ISqModelReader<SgAttributeTypeId, TblAttribute> GetReader()
        {
            return SgAttributeTypeIdReader.Instance;
        }

        private class SgAttributeTypeIdReader : ISqModelReader<SgAttributeTypeId, TblAttribute>
        {
            public static SgAttributeTypeIdReader Instance { get; } = new SgAttributeTypeIdReader();
            IReadOnlyList<ExprColumn> ISqModelReader<SgAttributeTypeId, TblAttribute>.GetColumns(TblAttribute table)
            {
                return SgAttributeTypeId.GetColumns(table);
            }

            SgAttributeTypeId ISqModelReader<SgAttributeTypeId, TblAttribute>.Read(ISqDataRecordReader record, TblAttribute table)
            {
                return SgAttributeTypeId.Read(record, table);
            }

            SgAttributeTypeId ISqModelReader<SgAttributeTypeId, TblAttribute>.ReadOrdinal(ISqDataRecordReader record, TblAttribute table, int offset)
            {
                return SgAttributeTypeId.ReadOrdinal(record, table, offset);
            }
        }

        public static ISqModelUpdaterKey<SgAttributeTypeId, TblAttribute> GetUpdater()
        {
            return SgAttributeTypeIdUpdater.Instance;
        }

        private class SgAttributeTypeIdUpdater : ISqModelUpdaterKey<SgAttributeTypeId, TblAttribute>
        {
            public static SgAttributeTypeIdUpdater Instance { get; } = new SgAttributeTypeIdUpdater();
            IRecordSetterNext ISqModelUpdater<SgAttributeTypeId, TblAttribute>.GetMapping(IDataMapSetter<TblAttribute, SgAttributeTypeId> dataMapSetter)
            {
                return SgAttributeTypeId.GetMapping(dataMapSetter);
            }

            IRecordSetterNext ISqModelUpdaterKey<SgAttributeTypeId, TblAttribute>.GetUpdateKeyMapping(IDataMapSetter<TblAttribute, SgAttributeTypeId> dataMapSetter)
            {
                return SgAttributeTypeId.GetUpdateKeyMapping(dataMapSetter);
            }

            IRecordSetterNext ISqModelUpdaterKey<SgAttributeTypeId, TblAttribute>.GetUpdateMapping(IDataMapSetter<TblAttribute, SgAttributeTypeId> dataMapSetter)
            {
                return SgAttributeTypeId.GetUpdateMapping(dataMapSetter);
            }
        }

        private class SgAttributeNameAndTypeReader : ISqModelReader<SgAttributeTypeId, TblAttribute>
        {
            public static SgAttributeNameAndTypeReader Instance { get; } = new SgAttributeNameAndTypeReader();
            IReadOnlyList<ExprColumn> ISqModelReader<SgAttributeTypeId, TblAttribute>.GetColumns(TblAttribute table)
            {
                return SgAttributeTypeId.GetColumns(table);
            }

            SgAttributeTypeId ISqModelReader<SgAttributeTypeId, TblAttribute>.Read(ISqDataRecordReader record, TblAttribute table)
            {
                return SgAttributeTypeId.Read(record, table);
            }

            SgAttributeTypeId ISqModelReader<SgAttributeTypeId, TblAttribute>.ReadOrdinal(ISqDataRecordReader record, TblAttribute table, int offset)
            {
                return SgAttributeTypeId.ReadOrdinal(record, table, offset);
            }
        }

        private class SgAttributeNameAndTypeUpdater : ISqModelUpdaterKey<SgAttributeTypeId, TblAttribute>
        {
            public static SgAttributeNameAndTypeUpdater Instance { get; } = new SgAttributeNameAndTypeUpdater();
            IRecordSetterNext ISqModelUpdater<SgAttributeTypeId, TblAttribute>.GetMapping(IDataMapSetter<TblAttribute, SgAttributeTypeId> dataMapSetter)
            {
                return SgAttributeTypeId.GetMapping(dataMapSetter);
            }

            IRecordSetterNext ISqModelUpdaterKey<SgAttributeTypeId, TblAttribute>.GetUpdateKeyMapping(IDataMapSetter<TblAttribute, SgAttributeTypeId> dataMapSetter)
            {
                return SgAttributeTypeId.GetUpdateKeyMapping(dataMapSetter);
            }

            IRecordSetterNext ISqModelUpdaterKey<SgAttributeTypeId, TblAttribute>.GetUpdateMapping(IDataMapSetter<TblAttribute, SgAttributeTypeId> dataMapSetter)
            {
                return SgAttributeTypeId.GetUpdateMapping(dataMapSetter);
            }
        }
    }
}