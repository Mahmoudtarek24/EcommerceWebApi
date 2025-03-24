
namespace Bussines_Logic.DTO.DaymentDto
{
	public class PaymentRequestDTO
	{
		public int CustomerId { get; set; }	
		public int OrderId { get; set; }
		public string PaymentMethod { get; set; } = null!;

		//[Required(ErrorMessage = Error.RequiredFiled)]
		//[Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
		//public decimal Amount { get; set; }

	}
}
