
namespace Bussines_Logic.ResponseDTO.ProductRespondDto
{
	public class ProductResponseDTO :IDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal Price { get; set; }
		public int StockQuantity { get; set; }
		public string ImageUrl { get; set; }
		public int DiscountPercentage { get; set; }
		public int CategoryId { get; set; }
		public bool IsAvailable { get; set; }
		public List<string> RelatedImages { get; set; }	=new List<string>();	
		public string message { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime? LastUpdateOn { get; set; }

	}
}
