
namespace Data_Access_Layer.Models
{
	public class Refund
	{
		public int RefundId { get; set; }
		public decimal Amount { get; set; }
		[Required]
		public RefundStatus Status { get; set; }
		[Required]
		public string RefundMethod { get; set; }
		[MaxLength(500)]	
		public string? RefundReason { get; set; }
		public string? TransactionId { get; set; }

		[Required]
		public DateTime InitiatedAt { get; set; }
		public DateTime? CompletedAt { get; set; }
		public int? ProcessedBy { get; set; }

		public int PaymentId { get; set; }	
		public Payment payment { get; set; }	

		public int CancellationId { get; set; }	
		public Cancellation cancellation { get; set; }	
	}
}
