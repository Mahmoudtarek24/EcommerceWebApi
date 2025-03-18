
namespace Bussines_Logic.DTO.AddressDto
{
	public class AddressDeleteDTO
	{
		[Required(ErrorMessage = Error.RequiredFiled)]
		public int CustomerId { get; set; }
		[Required(ErrorMessage =Error.RequiredFiled)]
		public int AddressId { get; set; }
	}
}
