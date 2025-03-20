
namespace Bussines_Logic.DTO.ProductDto
{
	public class ProductStatusUpdateDTO :IDto
	{
		[Required(ErrorMessage = Error.RequiredFiled)]
		public int ProductId { get; set; }
		[Required(ErrorMessage = Error.RequiredFiled)]
		public bool IsAvailable { get; set; }
	}
}
