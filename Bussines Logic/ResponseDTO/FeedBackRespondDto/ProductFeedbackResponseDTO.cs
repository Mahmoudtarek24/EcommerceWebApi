
namespace Bussines_Logic.ResponseDTO.FeedBackRespondDto
{
	public class ProductFeedbackResponseDTO
	{
		public int ProductId { get; set; }
		public string ProductName { get; set; }
		public double AverageRating { get; set; }
		public List<CustomerFeedback> Feedbacks { get; set; }
	}
}
