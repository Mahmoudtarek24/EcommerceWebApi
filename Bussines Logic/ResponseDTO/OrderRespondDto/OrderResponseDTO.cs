
namespace Bussines_Logic.ResponseDTO.OrderRespondDto
{
	public class OrderResponseDTO
	{
		public int OrderId { get; set; }
		public string OrderNumber { get; set; } 
		public DateTime OrderDate { get; set; }
		public decimal TotalBaseAmount { get; set; }
		public decimal TotalDiscountAmount { get; set; }
		public decimal ShippingCost { get; set; }
		public decimal TotalAmount { get; set; }
		public OrderStatus OrderStatus { get; set; }
		public int BillingAddressId { get; set; }
		public int ShippingAddressId { get; set; }
		public int CustomerId { get; set; }
		public List<OrderItemRespondDto> orderItem { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime CreateOn { get; set; } 
		public DateTime? LastUpdateOn { get; set; }
		public string Message { get; set; }	
	}
}
