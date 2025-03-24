using Bussines_Logic.DTO.OrderDto;
using Bussines_Logic.ResponseDTO.OrderRespondDto;
using Bussines_Logic.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrderController : ControllerBase
	{
		private readonly OrderServices<IDto> orderServices;

		public OrderController(OrderServices<IDto> orderServices)
		{
			this.orderServices = orderServices;
		}

		[HttpPost("CreateOrder")]
		public async Task<IActionResult> CreateOrder(OrderCreateDTO dTO)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await orderServices.CreateAsync(dTO);
			if (result.StatusCode != 200)
				return BadRequest(result);

			return Ok(result);
		}

		[HttpGet("GetOrderById/{id:int}")]
		public async Task<IActionResult> GetOrderById(int id)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await orderServices.GetByIdAsync(id);
			if (result.StatusCode != 200)
				return BadRequest(result);

			return Ok(result);
		}

		[HttpGet("GetAllOrders")]
		public async Task<ActionResult> GetAllOrders()
		{
			var response = await orderServices.GetAllOrdersAsync();
			if (response.StatusCode != 200)
			{
				return StatusCode(response.StatusCode, response);
			}
			return Ok(response);
		}


		[HttpGet("GetOrdersByCustomer/{customerId:int}")]
		public async Task<ActionResult> GetAllOrders(int customerId)
		{
			var response = await orderServices.GetOrdersByCustomerAsync(customerId);
			if (response.StatusCode != 200)
			{
				return StatusCode(response.StatusCode, response);
			}
			return Ok(response);
		}

		[HttpPut("UpdateOrderStatus")]
		public async Task<IActionResult> UpdateOrderStatus([FromBody] OrderStatusUpdateDTO statusDto)
		{
			var response = await orderServices.UpdateOrderStatusAsync(statusDto);
			if (response.StatusCode != 200)
			{
				return StatusCode(response.StatusCode, response);
			}
			return Ok(response);
		}
	}
}
