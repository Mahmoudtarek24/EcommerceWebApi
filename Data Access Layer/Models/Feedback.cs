
namespace Data_Access_Layer.Models
{
	public class Feedback
	{
		public int FeedbackId { get; set; }
		public int CustomerId { get; set; }	
		public Customer Customer { get; set; }	
		public int ProductId { get; set; }	
		public Product Product { get; set; }
		[Column(TypeName = "decimal(3,1)")]
		public double Rating { get; set; }
		[MaxLength(1000)]	
		public string? Comment { get; set; }

		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }


	}
}
