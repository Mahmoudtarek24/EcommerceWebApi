using Bussines_Logic.DTO.AddressDto;
using Bussines_Logic.DTO.IDTO;
using Bussines_Logic.Services.Services;
using Data_Access_Layer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AddressController : ControllerBase
	{
		private readonly AddressService<IDto> addressService;

		public AddressController(AddressService<IDto> addressService)
		{
			this.addressService = addressService;
		}
		[HttpPost("CreateAddress")]
		public async Task<IActionResult> CreateAddress([FromBody] AddressCreateDTO dTO)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result=await  addressService.CreateAsync(dTO);
			if(result.StatusCode!=200)
				return BadRequest(result);	

			return Ok(result);	
		}
		[HttpGet("GetAddressById/{id:int}")]
		public async Task<IActionResult> GetAddressById(int id)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await addressService.GetByIdAsync(id);
			if (result.StatusCode != 200)
				return BadRequest(result);

			return Ok(result);
		}

		[HttpPut("EditAddress")]
		public async Task<IActionResult> EditAddress([FromBody] AddressUpdateDTO dTO)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await addressService.UpdateAsync(dTO);
			if (result.StatusCode != 200)
				return BadRequest(result);

			return Ok(result);
		}

		[HttpDelete("DeleteAddress/{CustomerId}/{AddressId}")]
		public async Task<IActionResult> DeleteAddress([FromRoute]AddressDeleteDTO dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var responsed = await addressService.DeleteAsync(dto);
			if (responsed.StatusCode != 200)
				return BadRequest(responsed);
			return Ok(responsed);
		}

		[HttpGet("GetAddressesByCustomer/{customerId}")]
		public async Task<IActionResult> GetAddressesByCustomer(int customerId)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var responsed = await addressService.GetAddressesByCustomerAsync(customerId);
			if (responsed.StatusCode != 200)
				return BadRequest(responsed);
			return Ok(responsed);
		}
	}
}
