
namespace Data_Access_Layer.Models
{
	public class ProductImage
	{
		public int ImageId { get; set; }	
		public int ProductId { get; set; }
		[MaxLength(1000)]
		public string? ImageName { get; set; }	
		public Product Product { get; set; }	
	}
}
