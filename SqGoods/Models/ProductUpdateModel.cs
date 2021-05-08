using System;
using System.ComponentModel.DataAnnotations;

namespace SqGoods.Models
{
    public class ProductUpdateModel : ProductCreateModel
    {
        [Required]
        public Guid Id { get; set; }
    }
}