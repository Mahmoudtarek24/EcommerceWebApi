
namespace Data_Access_Layer.Models
{
	public class Payment 
	{
		public int PaymentId { get; set; }
		public string PaymentMethod { get; set; } = null!;
		public string TransactionId { get; set; } = null!;
		public decimal Amount { get; set; }
		public DateTime PaymentDate { get; set; }
		public PaymentStatus PaymentStatus { get; set; }
		public int OrderId { get; set; }	
		public Order Order { get; set; }	
		public Refund refund { get; set; }	

	}
}
