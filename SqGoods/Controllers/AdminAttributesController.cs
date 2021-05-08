using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SqGoods.Models;
using SqGoods.Services;

namespace SqGoods.Controllers
{
    [ApiController]
    [Route("api/admin/attributes")]
    public class AdminAttributesController : ControllerBase
    {
        private readonly AttributesService _attributesService;

        public AdminAttributesController(AttributesService attributesService)
        {
            this._attributesService = attributesService;
        }

        [OpenApiOperation("adminAttributesGetPage")]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(AttributeMetaListModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Get(
            [Range(0, int.MaxValue)] int offset = 0, 
            [Range(1, int.MaxValue)] int pageSize = 10, 
            Guid? categoryId = null,
            bool inverseCategoryId = false)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            AttributeMetaListModel page = await this._attributesService.Find(offset, pageSize, categoryId, inverseCategoryId);

            return this.Ok(page);
        }

        [OpenApiOperation("adminAttributesGetById")]
        [HttpGet("{attributeId:Guid}")]
        [ProducesResponseType(200, Type = typeof(AttributeUpdateModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(Guid attributeId)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            var result = await this._attributesService.Get(new[] { attributeId });

            if (result.Count < 1)
            {
                return this.NotFound();
            }

            return this.Ok(result.Single());
        }

        [OpenApiOperation("adminAttributesPost")]
        [ProducesResponseType(200, Type = typeof(IReadOnlyList<AttributeCreateModel>))]
        [ProducesResponseType(400)]
        [HttpPost]
        public async Task<IActionResult> Post(IReadOnlyList<AttributeCreateModel> createModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            await this._attributesService.Create(createModel);

            return this.Ok();
        }

        [OpenApiOperation("adminAttributesPut")]
        [ProducesResponseType(200, Type = typeof(IReadOnlyList<AttributeUpdateModel>))]
        [ProducesResponseType(400)]
        [HttpPut]
        public async Task<IActionResult> Put(IReadOnlyList<AttributeUpdateModel> updateModel)
        {
            await this._attributesService.Update(updateModel);

            return this.Ok();
        }

        [OpenApiOperation("adminAttributesDelete")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpDelete]
        public async Task<IActionResult> Delete(IReadOnlyList<Guid> attributeIds)
        {
            await this._attributesService.Delete(attributeIds);

            return this.Ok();
        }

        [OpenApiOperation("adminAttributeItemsGetById")]
        [HttpGet("{attributeId:Guid}/items")]
        [ProducesResponseType(200, Type = typeof(IReadOnlyList<AttributeItemModel>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetItems(Guid attributeId)
        {
            var resp = await this._attributesService.GetItems(attributeId); 

            if (!resp.GetResult(out var items, out var errorData))
            {
                return errorData.ToActionResult(this);
            }
            return this.Ok(items);
        }

        [OpenApiOperation("adminAttributeItemsPost")]
        [HttpPost("{attributeId:Guid}/items")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PostItems([FromRoute]Guid attributeId, IReadOnlyList<AttributeItemModel> items)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var resp = await this._attributesService.UpdateItems(attributeId, items);

            if (!resp.IsSuccessful(out var error))
            {
                error.ToActionResult(this);
            }

            return this.Ok();
        }


        [OpenApiOperation("adminAttributeSwapFormPost")]
        [HttpPost("swapForm")]
        public async Task<IActionResult> PostSwapAttributesForm(IReadOnlyList<AttributeSwapFormModel> forms)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var resp = await this._attributesService.SwapAttributes(forms);
            return !resp.IsSuccessful(out var error) ? error.ToActionResult(this) : this.Ok();
        }
    }
}