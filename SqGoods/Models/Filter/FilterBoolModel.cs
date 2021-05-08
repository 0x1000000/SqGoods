using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using SqGoods.Infrastructure;

namespace SqGoods.Models.Filter
{
    [JsonConverter(typeof(TypeDiscriminationConverter<FilterBoolModel>))]
    [KnownType(typeof(FilterAndModel))]
    [KnownType(typeof(FilterOrModel))]
    [KnownType(typeof(FilterSelectPredicateModel))]
    [KnownType(typeof(FilterSetPredicateModel))]
    [KnownType(typeof(FilterBoolPredicateModel))]
    [KnownType(typeof(FilterIntBetweenPredicateModel))]
    public abstract class FilterBoolModel
    {
        public string Type => TypeTagStorage.GetTagByType(this.GetType(), typeof(FilterBoolModel));

        public abstract TRes Accept<TRes>(IFilterBoolModelVisitor<TRes> visitor);
    }

    [TypeTag("and")]
    public class FilterAndModel : FilterBoolModel
    {
        public IReadOnlyList<FilterBoolModel> Items { get; set; } = Array.Empty<FilterBoolModel>();

        public override TRes Accept<TRes>(IFilterBoolModelVisitor<TRes> visitor)
        {
            return visitor.CaseAnd(this);
        }
    }

    [TypeTag("or")]
    public class FilterOrModel : FilterBoolModel
    {
        public IReadOnlyList<FilterBoolModel> Items { get; set; } = Array.Empty<FilterBoolModel>();

        public override TRes Accept<TRes>(IFilterBoolModelVisitor<TRes> visitor)
        {
            return visitor.CaseOr(this);
        }
    }

    [JsonConverter(typeof(TypeDiscriminationConverter<FilterPredicateModel>))]
    public abstract class FilterPredicateModel : FilterBoolModel
    {
        public Guid AttributeId { get; set; }
    }

    [TypeTag("select")]
    public class FilterSelectPredicateModel : FilterPredicateModel
    {
        public Guid SelectValueId { get; set; }

        public override TRes Accept<TRes>(IFilterBoolModelVisitor<TRes> visitor)
        {
            return visitor.CaseSelectPredicate(this);
        }
    }

    [TypeTag("set")]
    public class FilterSetPredicateModel : FilterPredicateModel
    {
        public IReadOnlyList<Guid> SelectValueIds { get; set; } = Array.Empty<Guid>();

        public override TRes Accept<TRes>(IFilterBoolModelVisitor<TRes> visitor)
        {
            return visitor.CaseSetPredicate(this);
        }
    }

    [TypeTag("bool")]
    public class FilterBoolPredicateModel : FilterPredicateModel
    {
        public bool Value { get; set; }

        public override TRes Accept<TRes>(IFilterBoolModelVisitor<TRes> visitor)
        {
            return visitor.CaseBoolPredicate(this);
        }
    }

    [TypeTag("num_between")]
    public class FilterIntBetweenPredicateModel : FilterPredicateModel
    {
        public int? From { get; set; }

        public int? To { get; set; }

        public override TRes Accept<TRes>(IFilterBoolModelVisitor<TRes> visitor)
        {
            return visitor.CaseIntBetweenPredicate(this);
        }
    }
}