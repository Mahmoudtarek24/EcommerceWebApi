
namespace Bussines_Logic.DTO.AddressDto
{
	public class AddressUpdateDTO :IDto
	{
		[Required(ErrorMessage = Error.RequiredFiled)]
		public int AddressId { get; set; }

		[Required(ErrorMessage = Error.RequiredFiled)]
		public int CustomerId { get; set; }


		[Required(ErrorMessage = Error.RequiredFiled)]

		[Display(Name = "Address Line 1"), StringLength(100, ErrorMessage = Error.MaxLength)]
		public string AddressLine1 { get; set; }



		[Display(Name = "Address Line 2"), StringLength(100, ErrorMessage = Error.MaxLength)]
		public string? AddressLine2 { get; set; }


		[Required(ErrorMessage = Error.RequiredFiled)]
		[StringLength(100, ErrorMessage = Error.MaxLength)]
		public string Area { get; set; }


		[Required(ErrorMessage = Error.RequiredFiled)]
		[StringLength(50, ErrorMessage = Error.MaxLength)]
		public string State { get; set; }


		[Required(ErrorMessage = Error.RequiredFiled)]
		//	[RegularExpression(Regiex.postalCode, ErrorMessage = "Invalid Postal Code.")]
		public string PostalCode { get; set; }


		[Required(ErrorMessage = Error.RequiredFiled)]
		[StringLength(50, ErrorMessage = Error.MaxLength)]
		public string Governorate { get; set; }
	}
}
