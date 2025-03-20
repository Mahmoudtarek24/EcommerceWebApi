
namespace Bussines_Logic.DTO.CartDto
{
	public class AddToCartDTO
	{
		[Required(ErrorMessage = Error.RequiredFiled)]
		public int CustomerId { get; set; }
		[Required(ErrorMessage = Error.RequiredFiled)]
		public int ProductId { get; set; }
		[Required(ErrorMessage = Error.RequiredFiled)]
		[Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100.")]
		public int Quantity { get; set; }
	}
}
