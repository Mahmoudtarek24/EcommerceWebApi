
namespace Bussines_Logic.DTO.CancellationDto
{
	public class CancellationStatusUpdateDTO
	{
		[Required(ErrorMessage = Error.RequiredFiled)]
		public int CancellationId { get; set; }
		[Required]
		public CancellationStatus Status { get; set; }
		//  Admin ID who is processing the cancellation
		public int? ProcessedBy { get; set; }
		// Any cancellation charges that apply (if any)
		[Range(0, double.MaxValue, ErrorMessage = "Cancellation charges must be non-negative.")]
		public decimal? CancellationCharges { get; set; }
		[StringLength(500, ErrorMessage = Error.StringLent)]
		public string? Remarks { get; set; }
	}
}
