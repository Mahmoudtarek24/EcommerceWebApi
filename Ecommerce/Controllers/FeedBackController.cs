using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FeedBackController : ControllerBase
	{
		private readonly FeedBackServices feedBackServices;
		public FeedBackController(FeedBackServices feedBackServices)
		{
			this.feedBackServices = feedBackServices;
		}

		[HttpPost("SubmitFeedback")]
		public async Task<ActionResult> SubmitFeedback([FromBody] FeedBackCreateDTO feedbackCreateDTO)
		{
			var response = await feedBackServices.SubmitFeedbackAsync(feedbackCreateDTO);
			if (response.StatusCode != 200)
			{
				return StatusCode(response.StatusCode, response);
			}
			return Ok(response);
		}

		[HttpGet("GetFeedbackForProduct/{productId}")]
		public async Task<ActionResult<ApiResponse<ProductFeedbackResponseDTO>>> GetFeedbackForProduct(int productId)
		{
			var response = await feedBackServices.GetFeedbackForProduct(productId);
			if (response.StatusCode != 200)
			{
				return StatusCode(response.StatusCode, response);
			}
			return Ok(response);
		}

		[HttpGet("GetAllFeedback")]   //For Admin
		public async Task<ActionResult> GetAllFeedback()
		{
			var response = await feedBackServices.GetAllFeedbackAsync();
			if (response.StatusCode != 200)
			{
				return StatusCode(response.StatusCode, response);
			}
			return Ok(response);
		}

		[HttpPut("UpdateFeedback")]
		public async Task<ActionResult> UpdateFeedback([FromBody] FeedbackUpdateDTO feedbackUpdateDTO)
		{
			var response = await feedBackServices.UpdateFeedbackAsync(feedbackUpdateDTO);
			if (response.StatusCode != 200)
			{
				return StatusCode(response.StatusCode, response);
			}
			return Ok(response);
		}

		[HttpDelete("DeleteFeedback")]
		public async Task<ActionResult> DeleteFeedback([FromBody] FeedbackDeleteDTO feedbackDeleteDTO)
		{
			var response = await feedBackServices.DeleteFeedbackAsync(feedbackDeleteDTO);
			if (response.StatusCode != 200)
			{
				return StatusCode(response.StatusCode, response);
			}
			return Ok(response);
		}
	}
}
