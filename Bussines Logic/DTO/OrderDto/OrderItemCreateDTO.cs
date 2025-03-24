namespace Bussines_Logic.DTO.OrderDto
{
	public class OrderItemCreateDTO:IDto
	{
		[Required(ErrorMessage = Error.RequiredFiled)]
		public int ProductId { get; set; }
		[Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100.")]
		public int Quantity { get; set; }
	}
}
