
namespace Bussines_Logic.DTO.OrderDto
{
	public class OrderStatusUpdateDTO
	{
		[Required(ErrorMessage = Error.RequiredFiled)]
		public int OrderId { get; set; }
		[Required]
		public string OrderStatus { get; set; }
	}
}
