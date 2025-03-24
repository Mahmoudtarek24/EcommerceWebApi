
namespace Bussines_Logic.ResponseDTO.CancellationRespondDto
{
	public class CancellationRespondDTO
	{
		public int Id { get; set; }
		public int OrderId { get; set; }
		public string Reason { get; set; }
		public string Status { get; set; }
		public DateTime RequestedAt { get; set; }
		public DateTime? ProcessedAt { get; set; }
		public int? ProcessedBy { get; set; }
		public decimal OrderAmount { get; set; }
		public decimal? CancellationCharges { get; set; }
		public string? Remarks { get; set; }
		public string Message { get; set; }	
	}
}
