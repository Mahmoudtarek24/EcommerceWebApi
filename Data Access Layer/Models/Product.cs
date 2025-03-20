
namespace Data_Access_Layer.Models
{
	public class Product :BaseModal
	{
		public int ProductId { get; set; }
		public string ProductName { get; set; } = null!;
		[MaxLength(3000)]
		public string Description { get; set; } = null!;
		[MaxLength(1000)]
		public string mainProductImage { get; set; } = null!;
		public decimal Price { get; set; }
		public int StockQuantity { get; set; }
	    public int DiscountPercentage { get; set; }
		public bool IsAvailable { get; set; }
		public int CategoryId { get; set; }
		public Category Category { get; set; }

		public ICollection<ProductImage>? productImages { get; set;}	

		public ICollection<OrderItem> orderItems { get; set; }		
	}
}
