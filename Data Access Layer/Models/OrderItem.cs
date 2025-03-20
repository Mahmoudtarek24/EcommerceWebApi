
namespace Data_Access_Layer.Models
{
	public class OrderItem
	{
		public int OrderItemId { get; set; }
		public int Quantity { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal Discount { get; set; }
		public decimal TotalPrice { get; set; }


		public int OrderId { get; set; } 
		public Order order { get; set; }	


		public int ProductId { get; set; }	
		public Product product { get; set; }	

	}
}
