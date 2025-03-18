using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
		public async Task<IActionResult> RegisterCustomer([FromBody] CustomerRegistrationDTO customerDto)
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
		[HttpPost("EditCustome")]
		public async Task<IActionResult> EditCustome([FromBody] CustomerUpdateDTO Dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var responsed = await customerService.UpdateAsync(Dto);
			if (responsed.StatusCode != 200)
				return BadRequest(responsed);
			return Ok(responsed);
		}
		[HttpGet("GetCustomerById/{id}")]
		public async Task<IActionResult> EditCustome(int id)
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

	}
}
