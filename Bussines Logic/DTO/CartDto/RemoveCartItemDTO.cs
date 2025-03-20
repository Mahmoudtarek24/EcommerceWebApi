
namespace Bussines_Logic.DTO.CartDto
{
	public class RemoveCartItemDTO
	{
		[Required(ErrorMessage =Error.RequiredFiled)]
		public int CustomerId { get; set; }
		[Required(ErrorMessage = Error.RequiredFiled)]
		public int CartItemId { get; set; }
	}
}
