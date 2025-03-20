
namespace Bussines_Logic.ResponseDTO.CartShoppingDto
{
	public class CartItemResponseDTO
	{
		public int CartItemId { get; set; }
		public int ProductId { get; set; }
		public string ProductName { get; set; }
		public int Quantity { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal Discount { get; set; }
		public decimal TotalPrice { get; set; }
	}
}
