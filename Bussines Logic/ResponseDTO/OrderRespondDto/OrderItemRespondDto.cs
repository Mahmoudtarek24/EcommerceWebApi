
namespace Bussines_Logic.ResponseDTO.OrderRespondDto
{
	public class OrderItemRespondDto
	{
		public int OrderItemId { get; set; }
		public int Quantity { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal Discount { get; set; }
		public decimal TotalPrice { get; set; }
		public int ProductId { get; set; }
	}
}
