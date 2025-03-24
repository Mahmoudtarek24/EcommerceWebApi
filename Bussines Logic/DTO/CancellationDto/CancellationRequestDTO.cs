
namespace Bussines_Logic.DTO.CancellationDto
{
	public class CancellationRequestDTO
	{
		public int OrderId { get; set; }	
		public int CustomerId { get; set; }
		[MaxLength(500, ErrorMessage = Error.MaxLength)]
		public string Reason { get; set; } = null!;
	     
	}
}
