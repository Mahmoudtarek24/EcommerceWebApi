
namespace Bussines_Logic.DTO.FeedBackDto
{
	public class ProductFeedBack
	{
		public int ProduckId { get; set; }
		[Range(0,10 , ErrorMessage = Error.Rang)]
		public double Reating { get; set; }

		public string? Comment { get; set; }

	}
}
