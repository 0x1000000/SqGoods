using System;
using SqExpress;
using SqExpress.QueryBuilders.RecordSetter;
using SqGoods.DomainLogic.Tables;
using SqExpress.Syntax.Names;
using System.Collections.Generic;

namespace SqGoods.DomainLogic.Models
{
    public record SgAttribute
    {
        public SgAttribute(Guid id, string name, SgAttributeType type, string? unit)
        {
            this.Id = id;
            this.Name = name;
            this.Type = type;
            this.Unit = unit;
        }

        public static SgAttribute Read(ISqDataRecordReader record, TblAttribute table)
        {
            return new SgAttribute(id: table.AttributeId.Read(record), name: table.Name.Read(record), type: (SgAttributeType)table.Type.Read(record), unit: table.Unit.Read(record));
        }

        public static SgAttribute ReadOrdinal(ISqDataRecordReader record, TblAttribute table, int offset)
        {
            return new SgAttribute(id: table.AttributeId.Read(record, offset), name: table.Name.Read(record, offset + 1), type: (SgAttributeType)table.Type.Read(record, offset + 2), unit: table.Unit.Read(record, offset + 3));
        }

        public Guid Id { get; }

        public string Name { get; }

        public SgAttributeType Type { get; }

        public string? Unit { get; }

        public static TableColumn[] GetColumns(TblAttribute table)
        {
            return new TableColumn[]{table.AttributeId, table.Name, table.Type, table.Unit};
        }

        public static IRecordSetterNext GetMapping(IDataMapSetter<TblAttribute, SgAttribute> s)
        {
            return s.Set(s.Target.AttributeId, s.Source.Id).Set(s.Target.Name, s.Source.Name).Set(s.Target.Type, (short)s.Source.Type).Set(s.Target.Unit, s.Source.Unit);
        }

        public static IRecordSetterNext GetUpdateKeyMapping(IDataMapSetter<TblAttribute, SgAttribute> s)
        {
            return s.Set(s.Target.AttributeId, s.Source.Id);
        }

        public static IRecordSetterNext GetUpdateMapping(IDataMapSetter<TblAttribute, SgAttribute> s)
        {
            return s.Set(s.Target.Name, s.Source.Name).Set(s.Target.Type, (short)s.Source.Type).Set(s.Target.Unit, s.Source.Unit);
        }

        public static ISqModelReader<SgAttribute, TblAttribute> GetReader()
        {
            return SgAttributeReader.Instance;
        }

        private class SgAttributeReader : ISqModelReader<SgAttribute, TblAttribute>
        {
            public static SgAttributeReader Instance { get; } = new SgAttributeReader();
            IReadOnlyList<ExprColumn> ISqModelReader<SgAttribute, TblAttribute>.GetColumns(TblAttribute table)
            {
                return SgAttribute.GetColumns(table);
            }

            SgAttribute ISqModelReader<SgAttribute, TblAttribute>.Read(ISqDataRecordReader record, TblAttribute table)
            {
                return SgAttribute.Read(record, table);
            }

            SgAttribute ISqModelReader<SgAttribute, TblAttribute>.ReadOrdinal(ISqDataRecordReader record, TblAttribute table, int offset)
            {
                return SgAttribute.ReadOrdinal(record, table, offset);
            }
        }

        public static ISqModelUpdaterKey<SgAttribute, TblAttribute> GetUpdater()
        {
            return SgAttributeUpdater.Instance;
        }

        private class SgAttributeUpdater : ISqModelUpdaterKey<SgAttribute, TblAttribute>
        {
            public static SgAttributeUpdater Instance { get; } = new SgAttributeUpdater();
            IRecordSetterNext ISqModelUpdater<SgAttribute, TblAttribute>.GetMapping(IDataMapSetter<TblAttribute, SgAttribute> dataMapSetter)
            {
                return SgAttribute.GetMapping(dataMapSetter);
            }

            IRecordSetterNext ISqModelUpdaterKey<SgAttribute, TblAttribute>.GetUpdateKeyMapping(IDataMapSetter<TblAttribute, SgAttribute> dataMapSetter)
            {
                return SgAttribute.GetUpdateKeyMapping(dataMapSetter);
            }

            IRecordSetterNext ISqModelUpdaterKey<SgAttribute, TblAttribute>.GetUpdateMapping(IDataMapSetter<TblAttribute, SgAttribute> dataMapSetter)
            {
                return SgAttribute.GetUpdateMapping(dataMapSetter);
            }
        }
    }
}