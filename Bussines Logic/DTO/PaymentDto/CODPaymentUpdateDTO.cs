namespace Bussines_Logic.DTO.PaymentDto
{
	public class CODPaymentUpdateDTO
	{
		[Required(ErrorMessage = Error.RequiredFiled)]
		public int OrderId { get; set; }
		[Required(ErrorMessage = Error.RequiredFiled)]
		public int PaymentId { get; set; } 

	}
}
