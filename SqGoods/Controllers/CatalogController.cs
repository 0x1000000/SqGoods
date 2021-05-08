using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SqGoods.Infrastructure;
using SqGoods.Models;
using SqGoods.Models.Fields;
using SqGoods.Models.Filter;
using SqGoods.Services;

namespace SqGoods.Controllers
{
    [ApiController]
    [Route("api/catalog")]
    [KnownType(typeof(FilterBoolModel))]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogService _catalogService;

        public CatalogController(CatalogService catalogService)
        {
            this._catalogService = catalogService;
        }

        [OpenApiOperation("catalogGetTopCategories")]
        [HttpGet("top-categories")]
        public async Task<IEnumerable<IdNameModel>> TopCategories()
        {
            return await this._catalogService.GetTopCategories();
        }

        [OpenApiOperation("catalogGetCategoryProductPage")]
        [HttpGet("{categoryId:guid}")]
        [ProducesResponseType(200, Type = typeof(DataPageModel<ProductListModel>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetProducts(
            Guid categoryId,
            string? filterJson = null,
            [Range(0, int.MaxValue)] int offset = 0,
            [Range(1, int.MaxValue)] int pageSize = 10)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            FilterBoolModel? filter = null;
            if (filterJson != null)
            {
                try
                {
                    filter = JsonSerializer.Deserialize<FilterBoolModel>(filterJson, new JsonSerializerOptions(JsonSerializerDefaults.Web));
                }
                catch (TypeDiscriminationConverterException e)
                {
                    return this.BadRequest("Incorrect filter format: " + e.Message);
                }
            }

            var response = await this._catalogService.FindProducts(categoryId, offset, pageSize, filter);

            return response.GetResult(out var result, out var error) 
                ? this.Ok(result) 
                : error.ToActionResult(this);
        }

        [OpenApiOperation("catalogGetCategoryAttributes")]
        [HttpGet("{categoryId:guid}/categoryAttributes")]
        [ProducesResponseType(200, Type = typeof(IReadOnlyList<FieldDescriptorModel>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetCategoryAttributes(Guid categoryId)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var result = await this._catalogService.GetCategoryAttributes(categoryId);
            return this.Ok(result);
        }

        [OpenApiOperation("catalogPostFilterValidationForm")]
        [HttpPost("{categoryId:guid}/categoryAttributes")]
        public string PostFilterValidationForm(FilterBoolModel filter)
        {
            return "";
        }
    }
}