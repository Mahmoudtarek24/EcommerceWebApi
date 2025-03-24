
namespace Bussines_Logic.DTO.DaymentDto
{
	public class PaymentStatusUpdateDTO
	{
		[Required(ErrorMessage = Error.RequiredFiled)]
		public int PaymentId { get; set; }
		public string? TransactionId { get; set; }

		[Required(ErrorMessage = Error.RequiredFiled)]
		public PaymentStatus Status { get; set; } 
	}
}
