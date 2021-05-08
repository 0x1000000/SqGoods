using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SqGoods.Models;
using SqGoods.Services;

namespace SqGoods.Controllers
{
    [ApiController]
    [Route("api/admin/products")]
    public class AdminProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public AdminProductsController(ProductService productService)
        {
            this._productService = productService;
        }

        [OpenApiOperation("adminProductsGetPage")]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(DataPageModel<ProductListModel>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Get(
            [Range(0, int.MaxValue)] int offset = 0,
            [Range(1, int.MaxValue)] int pageSize = 10,
            Guid? categoryId = null)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var response = await this._productService.Find(offset, pageSize, categoryId, null);

            return response.GetResult(out var result, out var error) 
                ? this.Ok(result) 
                : error.ToActionResult(this);
        }

        [OpenApiOperation("adminProductsGet")]
        [HttpGet("{productId:guid}")]
        [ProducesResponseType(200, Type = typeof(ProductUpdateModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Get(Guid productId)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var response = await this._productService.Get(new[] { productId });

            if (!response.GetResult(out var result, out var error))
            {
                return error.ToActionResult(this);
            }

            if (result.Count < 1)
            {
                return this.NotFound();
            }

            return this.Ok(result[0]);
        }

        [OpenApiOperation("adminProductsGetCategories")]
        [HttpGet("categories")]
        [ProducesResponseType(200, Type = typeof(IReadOnlyList<IdNameModel>))]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await this._productService.GetCategories();
            return this.Ok(categories);
        }

        [OpenApiOperation("adminProductsPost")]
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(IReadOnlyList<ProductCreateModel>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post([MinLength(1)] IReadOnlyList<ProductCreateModel> products)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var resp = await this._productService.Create(products);
            if (!resp.IsSuccessful(out var error))
            {
                return error.ToActionResult(this);
            }

            return this.Ok();
        }

        [OpenApiOperation("adminProductsPut")]
        [HttpPut]
        [ProducesResponseType(200, Type = typeof(IReadOnlyList<ProductUpdateModel>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Put([MinLength(1)] IReadOnlyList<ProductUpdateModel> products)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var resp = await this._productService.Update(products);
            if (!resp.IsSuccessful(out var error))
            {
                return error.ToActionResult(this);
            }

            return this.Ok();
        }
    }
}