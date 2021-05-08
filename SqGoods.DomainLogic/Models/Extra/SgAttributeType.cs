using System;
using System.Runtime.Serialization;

namespace SqGoods.DomainLogic.Models
{
    public enum SgAttributeType
    {
        Boolean,
        Integer,
        Select,
        SubSet
    }

    public interface ISgAttributeTypeVisitor<out TRes>
    {
        TRes CaseBoolean();
        TRes CaseInteger();
        TRes CaseSelect();
        TRes CaseSubset();
    }

    public static class SgAttributeTypeExtensions
    {
        public static TRes Switch<TVisitor, TRes>(this SgAttributeType attributeType, ref TVisitor visitor)
            where TVisitor : struct, ISgAttributeTypeVisitor<TRes>
        {
            switch (attributeType)
            {
                case SgAttributeType.Boolean:
                    return visitor.CaseBoolean();
                case SgAttributeType.Integer:
                    return visitor.CaseInteger();
                case SgAttributeType.Select:
                    return visitor.CaseSelect();
                case SgAttributeType.SubSet:
                    return visitor.CaseSubset();
                default:
                    throw new ArgumentOutOfRangeException(nameof(attributeType), attributeType, null);
            }
        }

        public static bool IsSelectSet(this SgAttributeType attributeType) =>
            attributeType == SgAttributeType.SubSet || attributeType == SgAttributeType.Select;
    }
}