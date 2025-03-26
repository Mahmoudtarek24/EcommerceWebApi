
namespace Bussines_Logic.DTO.FeedBackDto
{
	public class FeedBackCreateDTO
	{
		[Required(ErrorMessage =Error.RequiredFiled)]
		public int CustomerId { get; set; }

		public int ProductId { get; set; }
		[Range(0, 10, ErrorMessage = Error.Rang)]
		public double Reting { get; set; }

		public string? Comment { get; set; }

	}
}
