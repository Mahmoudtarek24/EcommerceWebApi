
namespace Bussines_Logic.ResponseDTO.FeedBackRespondDto
{
	public class FeedbackResponseDTO
	{
		public int Id { get; set; }
		public int CustomerId { get; set; }
		public string CustomerName { get; set; } // Combines FirstName and LastName
		public int ProductId { get; set; }
		public string ProductName { get; set; }
		public double Rating { get; set; }
		public string? Comment { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
	}
}
