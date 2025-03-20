
namespace Bussines_Logic.DTO.OrderDto
{
	public class OrderCreateDTO 
	{
		[Required(ErrorMessage = Error.RequiredFiled)]
		public int CustomerId { get; set; }


		[Required(ErrorMessage =  Error.RequiredFiled)]
		public int BillingAddressId { get; set; }


		[Required(ErrorMessage = Error.RequiredFiled)]
		public int ShippingAddressId { get; set; }


		[Required(ErrorMessage = Error.RequiredFiled)]
		[MinLength(1, ErrorMessage = "At least one order item is required.")]
		public List<OrderItemCreateDTO> OrderItems { get; set; } = new List<OrderItemCreateDTO>(); 
	}
}
