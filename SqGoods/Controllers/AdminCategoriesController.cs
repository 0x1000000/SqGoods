using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SqGoods.Models;
using SqGoods.Models.Fields;
using SqGoods.Services;

namespace SqGoods.Controllers
{
    [ApiController]
    [Route("api/admin/categories")]
    public class AdminCategoriesController : ControllerBase
    {
        private readonly CategoriesService _categoriesService;

        public AdminCategoriesController(CategoriesService categoriesService)
        {
            this._categoriesService = categoriesService;
        }

        [OpenApiOperation("adminCategoriesGetPage")]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(DataPageModel<CategoryListModel>))]
        public async Task<DataPageModel<CategoryListModel>> Get(int offset, int pageSize)
        {
            return await this._categoriesService.Find(offset, pageSize);
        }

        [OpenApiOperation("adminCategoriesGetById")]
        [HttpGet("{categoryId:guid}")]
        [ProducesResponseType(200, Type = typeof(CategoryUpdateModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(Guid categoryId)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var result = await this._categoriesService.Get(new []{ categoryId });

            if (result.Count < 1)
            {
                return this.NotFound();
            }

            return this.Ok(result.Single());
        }

        [OpenApiOperation("adminCategoriesPost")]
        [ProducesResponseType(200, Type = typeof(IReadOnlyList<CategoryUpdateModel>))]
        [ProducesResponseType(400)]
        [HttpPost]
        public async Task<IActionResult> Post(IReadOnlyList<CategoryCreateModel> categories)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            return this.Ok(await this._categoriesService.Create(categories));
        }

        [OpenApiOperation("adminCategoriesPut")]
        [ProducesResponseType(200, Type = typeof(IReadOnlyList<CategoryUpdateModel>))]
        [ProducesResponseType(400)]
        [HttpPut]
        public async Task<IActionResult> Put(IReadOnlyList<CategoryUpdateModel> categories)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            await this._categoriesService.Update(categories);

            return this.Ok();
        }

        [OpenApiOperation("adminCategoriesDelete")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpDelete]
        public async Task<IActionResult> Delete(IReadOnlyList<Guid> categoryIds)
        {
            await this._categoriesService.Delete(categoryIds);

            return this.Ok();
        }

        [OpenApiOperation("adminCategoriesGetFields")]
        [ProducesResponseType(200, Type = typeof(IReadOnlyList<FormFieldModel>))]
        [ProducesResponseType(404)]
        [HttpGet]
        [Route("{categoryId:guid}/fields")]
        public async Task<IActionResult> GetFields(Guid categoryId)
        {
            var result = await this._categoriesService.GetFormFields(categoryId);

            return this.Ok(result);
        }

        [OpenApiOperation("adminCategoriesPostAttributes")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [HttpPost]
        [Route("{categoryId:guid}/attributes")]
        public async Task<IActionResult> PostAttributes(Guid categoryId, IReadOnlyList<Guid> attributes)
        {
            var result = await this._categoriesService.AddCategories(categoryId, attributes);
            if (!result.IsSuccessful(out var error))
            {
                return error.ToActionResult(this);
            }
            return this.Ok();
        }

    }
}