using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
//	[Authorize]
	public class ProductController : ControllerBase
	{
		private readonly ProductServices<IDto> productServices;

		public ProductController(ProductServices<IDto> productServices)
		{
			this.productServices = productServices;
		}
		[HttpPost("CreateProduct")]
		public async Task<IActionResult> CreateProduct([FromForm] ProductCreateDTO dTO)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await productServices.CreateAsync(dTO);
			if (result.StatusCode != 200)
				return BadRequest(result);

			return Ok(result);
		}

		//[ProducesResponseType(StatusCodes.Status200OK)]	
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]	
		//[ProducesResponseType(StatusCodes.Status403Forbidden)]	
		//[ProducesResponseType(StatusCodes.Status500InternalServerError)]	
		[HttpGet("GetProductById/{id:int}")]
		public async Task<IActionResult> GetProductById(int id)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await productServices.GetByIdAsync(id);
			if (result.StatusCode != 200)
				return BadRequest(result);

			return Ok(result);
		}

		[HttpGet("GetProductsByCategoryId/{categoryId}")]
		public async Task<IActionResult> GetroductsByCategory(int categoryId)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var responsed = await productServices.GetAllProductsByCategoryAsync(categoryId);
			if (responsed.StatusCode != 200)
				return BadRequest(responsed);
			return Ok(responsed);
		}

		[HttpPut("EditProducr")]
		public async Task<IActionResult> EditAddress([FromForm] ProductUpdateDTO dTO)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await productServices.UpdateAsync(dTO);
			if (result.StatusCode != 200)
				return BadRequest(result);

			return Ok(result);
		}

		[HttpDelete("Delete/{id}")]
		public async Task<IActionResult> DeleteProduct(int id)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var responsed = await productServices.DeleteAsync(id);
			if (responsed.StatusCode != 200)
				return BadRequest(responsed);
			return Ok(responsed);
		}


		[HttpPut("UpdateProductStatus")]
		public async Task<ActionResult> UpdateProductStatus(ProductStatusUpdateDTO productStatusUpdateDTO)
		{
			var response = await productServices.UpdateProductStatusAsync(productStatusUpdateDTO);
			if (response.StatusCode != 200)
			{
				return StatusCode(response.StatusCode, response);
			}
			return Ok(response);
		}
	}
}
