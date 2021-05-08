using System;
using System.Collections.Generic;
using SqGoods.DomainLogic.Models;

namespace SqGoods.Models
{
    public class AttributeNameModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}