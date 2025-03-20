
namespace Data_Access_Layer.Models
{
	public class CartItem
	{
		public int CartItemId { get; set; }	
		public int ProductId { get; set; }	
		public Product Product { get; set; }

		public int Quantity { get; set; }
		public decimal UnitPrice { get; set; } //product price
		public decimal Discount { get; set; }            /// like tottal didcount =234
		public decimal TotalPrice { get; set; }  ///tottal price for only one product(quentity*price)
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }

		public int CartId { get; set; }
		public Cart Cart { get; set; }
	}
}
