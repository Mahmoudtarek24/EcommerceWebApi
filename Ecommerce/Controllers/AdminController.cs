using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AdminController : ControllerBase
	{
		private readonly AdminServices adminServices;
		public AdminController(AdminServices adminServices )
		{
			this.adminServices = adminServices;	
		}
		[HttpGet("AllCustomers")]
		public async Task<IActionResult> AllCustomers()
		{
			var respond = await adminServices.GetAllCustomer();
			if (respond.StatusCode!=200){
				return StatusCode(respond.StatusCode,respond);
			}

			return Ok(respond);
		}
	}
}
