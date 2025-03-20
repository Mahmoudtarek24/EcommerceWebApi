using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CartsController : ControllerBase
	{
		private readonly ShoppingCartService shoppingCartService;
		public CartsController(ShoppingCartService shoppingCartService)
		{
			this.shoppingCartService = shoppingCartService;
		}

		[HttpGet("GetCart/{customerId}")]
		public async Task<ActionResult<CartResponseDTO>> GetCartByCustomerId(int customerId)
		{
			var response = await shoppingCartService.GetCartByCustomerIdAsync(customerId);
			if (response.StatusCode != 200)
			{
				return StatusCode(response.StatusCode, response);
			}
			return Ok(response);
		}

		[HttpPost("AddToCart")]
		public async Task<IActionResult> AddToCart([FromBody]AddToCartDTO Dto)
		{
			if(!ModelState.IsValid) 
				return BadRequest(ModelState);		

			var result=await shoppingCartService.AddToCartAsync(Dto);	
			if(result.StatusCode!=200)
				return BadRequest(result);

			return Ok(result);		

		}
		[HttpPost("UpdateCartItem")]
		public async Task<IActionResult> UpdateCartItemDTO([FromBody]UpdateCartItemDTO Dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await shoppingCartService.UpdateCartItemAsync(Dto);
			if (result.StatusCode != 200)
				return BadRequest(result);

			return Ok(result);

		}

		[HttpDelete("DeleteCartItem")]
		public async Task<IActionResult> DeleteCartItem([FromBody] RemoveCartItemDTO Dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await shoppingCartService.RemoveCartItemAsync(Dto);
			if (result.StatusCode != 200)
				return BadRequest(result);

			return Ok(result);

		}
		[HttpDelete("ClearCart")]
		public async Task<ActionResult<ConfirmationResponseDTO>> ClearCart([FromQuery] int customerId)
		{
			var response = await shoppingCartService.ClearCartAsync(customerId);
			if (response.StatusCode != 200)
			{
				return StatusCode(response.StatusCode, response);
			}
			return Ok(response);
		}
	}
}
