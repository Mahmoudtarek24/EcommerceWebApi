
namespace Data_Access_Layer.Models
{
	public class Cancellation
	{
		public int CancellationId { get; set; }
		[MaxLength(500)]	
		public string Reason { get; set; }
	    public CancellationStatus cancellationStatus { get; set; }
		public DateTime RequestedAt { get; set; }
		public DateTime? ProcessedAt { get; set; }

		public int? ProcessedBy { get; set; }  //will bind it with admine
		public decimal OrderAmount { get; set; }
		public decimal? CancellationCharges { get; set; } = 0.00m;
		[MaxLength(500)]
		public string? Remarks { get; set; }
		public int OrderId { get; set; }
		public Order Order { get; set; }
		public Refund refund { get; set; }
	}
}
