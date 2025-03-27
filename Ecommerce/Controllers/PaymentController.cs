using Bussines_Logic.DTO.PaymentDto;
using Bussines_Logic.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentController : ControllerBase
	{
		private readonly PaymentServices paymentServices;
		public PaymentController(PaymentServices paymentServices)
		{
			this.paymentServices = paymentServices;
		}

		[HttpPost("ProcessPayment")]
		public async Task<ActionResult> ProcessPayment([FromBody] PaymentRequestDTO paymentRequest)
		{
			var response = await paymentServices.ProcessPaymentAsync(paymentRequest);
			if (response.StatusCode != 200)
			{
				return StatusCode(response.StatusCode, response);
			}
			return Ok(response);
		}

		[HttpGet("GetPaymentById/{paymentId}")]
		public async Task<ActionResult> GetPaymentById(int paymentId)
		{
			var response = await paymentServices.GetPaymentByIdAsync(paymentId);
			if (response.StatusCode != 200)
			{
				return StatusCode(response.StatusCode, response);
			}
			return Ok(response);
		}

		[HttpGet("GetPaymentByOrderId/{orderId}")]
		public async Task<ActionResult> GetPaymentByOrderId(int orderId)
		{
			var response = await paymentServices.GetPaymentByOrderIdAsync(orderId);
			if (response.StatusCode != 200)
			{
				return StatusCode(response.StatusCode, response);
			}
			return Ok(response);
		}

		[HttpPost("CompleteCODPayment")]
		public async Task<ActionResult> CompleteCODPayment([FromBody] CODPaymentUpdateDTO codPaymentUpdateDTO)
		{
			var response = await paymentServices.CompleteCODPaymentAsync(codPaymentUpdateDTO);
			if (response.StatusCode != 200)
			{
				return StatusCode(response.StatusCode, response);
			}
			return Ok(response);
		}

		[HttpPost("BuyingCartItems")]
		public async Task<ActionResult> ProcessCart([FromBody]buyingCartItemsDTO dTO)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var respond=await paymentServices.ProcessCartAsync(dTO);	
			if(respond.StatusCode!=200)
				return StatusCode(respond.StatusCode, respond);	

			return Ok(respond);
		}
	}
}
