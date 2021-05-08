using System;

namespace SqGoods.DomainLogic.Models
{
    public interface ISgCategoryOrder
    {
        int Order { get; }

        int? TopOrder { get; }
    }

    public interface ISgCategoryIdentity
    {
        Guid Id { get; }
    }
}