using System;

namespace SqGoods.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TypeTagAttribute : Attribute
    {
        public string TypeTag { get; }

        public TypeTagAttribute(string typeTag)
        {
            this.TypeTag = typeTag;
        }
    }
}