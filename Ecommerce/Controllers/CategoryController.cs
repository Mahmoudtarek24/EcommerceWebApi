using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CategoryController : ControllerBase
	{
		private readonly CategoryServices<IDto> categoryServices;

		public CategoryController(CategoryServices<IDto> categoryServices)
		{
			this.categoryServices = categoryServices;
		}

		[HttpPost("Create")]
		public async Task<IActionResult> Create(CategoryCreateDTO dTO)
		{
			if(!ModelState.IsValid)
				return BadRequest(ModelState);		

			var result=await categoryServices.CreateAsync(dTO);
			if(result.StatusCode!=200)
				return BadRequest(result);
			
			return Ok(result);
		}

		[HttpDelete("Delete/{id}")]
		public async Task<IActionResult> DeleteCategory(int id)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var responsed = await categoryServices.DeleteAsync(id);
			if (responsed.StatusCode != 200)
				return BadRequest(responsed);
			return Ok(responsed);
		}
		[HttpGet]
		public async Task<IActionResult> GetAllCategory()
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var responsed = await categoryServices.GetAllCategory();
			if (responsed.StatusCode != 200)
				return BadRequest(responsed);

			return Ok(responsed);
		}
		[HttpPut("EditCategory")]
		public async Task<IActionResult> EditCategory([FromBody] CategoryUpdateDTO dTO)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await categoryServices.UpdateAsync(dTO);
			if (result.StatusCode != 200)
				return BadRequest(result);

			return Ok(result);
		}

		[HttpGet("GetCategoryById/{id:int}")]
		public async Task<IActionResult> GetCategoryById(int id)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await categoryServices.GetByIdAsync(id);
			if (result.StatusCode != 200)
				return BadRequest(result);

			return Ok(result);
		}

	}
}
