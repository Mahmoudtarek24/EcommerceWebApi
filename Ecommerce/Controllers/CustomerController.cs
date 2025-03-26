using Bussines_Logic.DTO.CustomerDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CustomerController : ControllerBase
	{
		private readonly CustomerService<IDto> customerService;

		public CustomerController(CustomerService<IDto> customerService)
		{
			this.customerService = customerService;	
		}

		[HttpPost("Register")]
		public async Task<IActionResult> RegisterCustomer([FromForm] CustomerRegistrationDTO customerDto)
		{
			if(!ModelState.IsValid) 
       			return BadRequest(ModelState);
				
			var responsed=await customerService.CreateAsync(customerDto);	
			if(responsed.StatusCode!=200)
				return BadRequest(responsed);	
			return Ok(responsed);	
		}
		[HttpPost("Login")]
		public async Task<IActionResult> LoginCustoner([FromBody] LoginDTO Dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var responsed = await customerService.LoginAsync(Dto);
			if (responsed.StatusCode != 200)
				return BadRequest(responsed);
			return Ok(responsed);
		}
		[HttpPost("ChangePassword")]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO Dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var responsed = await customerService.ChangePasswordAsync(Dto);
			if (responsed.StatusCode != 200)
				return BadRequest(responsed);
			return Ok(responsed);
		}
		[HttpPost("EditCustomer")]
		public async Task<IActionResult> EditCustomer([FromForm] CustomerUpdateDTO Dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var responsed = await customerService.UpdateAsync(Dto);
			if (responsed.StatusCode != 200)
				return BadRequest(responsed);
			return Ok(responsed);
		}
		[HttpGet("GetCustomerById/{id}")]
		public async Task<IActionResult> CustomerById(int id)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var responsed = await customerService.GetByIdAsync(id);
			if (responsed.StatusCode != 200)
				return BadRequest(responsed);
			return Ok(responsed);
		}
		[HttpDelete("DeleteCustomer/{id}")]
		public async Task<IActionResult> DeleteCustomer(int id)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var responsed = await customerService.DeleteAsync(id);
			if (responsed.StatusCode != 200)
				return BadRequest(responsed);
			return Ok(responsed);
		}
		[HttpPost("ForgotPassword")]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
		{

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var respond=await customerService.ForgotPasswordAsync(dto);	
			if(respond.StatusCode!=200)
				return StatusCode(respond.StatusCode,respond);

			return Ok(respond);	

		}
		[HttpPost("ResetPassword")]////123456789Mm#
		public async Task<IActionResult> ResetPassword(ResetPasswordDTO dTO)
		{
			if(!ModelState.IsValid) 
				return BadRequest(ModelState);

			var respond=await customerService.ResetPasswordAsync(dTO);	
			if(respond.StatusCode!=200)
				return StatusCode(respond.StatusCode,respond);
			
			return Ok(respond);	
		}
		
	}
}
