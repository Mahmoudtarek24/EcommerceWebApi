using Bussines_Logic.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CancellationController : ControllerBase
	{
		private readonly CancellationServices cancellationServices;
		public CancellationController(CancellationServices cancellationServices)
		{
			this.cancellationServices = cancellationServices;
		}

		[HttpPost("RequestCancellation")]
		public async Task<IActionResult> RequestCancellation([FromBody] CancellationRequestDTO dTO)
		{
          var result=await cancellationServices.RequestCancellationAsync(dTO);	
			if(result.StatusCode!=200)
				return StatusCode(result.StatusCode,result);
			return Ok(result);
		}

		[HttpGet("GetAllCancellations")]
		public async Task<ActionResult> GetAllCancellations()
		{
			var response = await cancellationServices.GetAllCancellationsAsync();
			if (response.StatusCode != 200)
			{
				return StatusCode(response.StatusCode, response);
			}
			return Ok(response);
		}

		[HttpGet("GetCancellationById/{id}")]
		public async Task<ActionResult> GetCancellationById(int id)
		{
			var response = await cancellationServices.GetCancellationById(id);
			if (response.StatusCode != 200)
			{
				return StatusCode(response.StatusCode, response);
			}
			return Ok(response);
		}

		[HttpPut("UpdateCancellationStatus")]
		public async Task<ActionResult> UpdateCancellationStatus([FromBody] CancellationStatusUpdateDTO statusUpdate)
		{
			var response = await cancellationServices.UpdateCancellationStatusAsync(statusUpdate);
			if (response.StatusCode != 200)
			{
				return StatusCode(response.StatusCode, response);
			}
			return Ok(response);
		}
	}
}
