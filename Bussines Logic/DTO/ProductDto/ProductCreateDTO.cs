
namespace Bussines_Logic.DTO.ProductDto
{
	public class ProductCreateDTO :IDto
	{
		[Required(ErrorMessage = Error.RequiredFiled)]
		[StringLength(100, MinimumLength = 3, ErrorMessage = Error.StringLent)]
		public string Name { get; set; }


		[Required(ErrorMessage = Error.RequiredFiled)]
		[MinLength(10, ErrorMessage = Error.MinLength)]
		public string Description { get; set; }


		[Range(0.01, 100000.00, ErrorMessage = Error.RangValue)]
		public decimal Price { get; set; }


		[Range(0, 3000, ErrorMessage = Error.RangValue)]
		public int StockQuantity { get; set; }


		[Range(0, 100, ErrorMessage = Error.RangValue)]
		public int DiscountPercentage { get; set; } = 0;


		[Required(ErrorMessage = Error.RequiredFiled)]
		public int CategoryId { get; set; }

		public IFormFile MainProductIamge { get; set; }

		public List<IFormFile>? ProductImages { get; set; }	
	}
}
