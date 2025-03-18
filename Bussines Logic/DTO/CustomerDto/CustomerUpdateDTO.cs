
namespace Bussines_Logic.DTO
{
	public class CustomerUpdateDTO :IDto
	{
		[Required(ErrorMessage = Error.RequiredFiled)]
		public int CustomerId { get; set; }
		[Required(ErrorMessage = Error.RequiredFiled)]
		[MinLength(2, ErrorMessage = Error.MinLength)]
		[MaxLength(50, ErrorMessage = Error.MaxLength)]
		public string FirstName { get; set; }


		[Required(ErrorMessage = Error.RequiredFiled)]
		[MinLength(2, ErrorMessage = Error.MinLength)]
		[MaxLength(50, ErrorMessage = Error.MaxLength)]
		public string LastName { get; set; }

		[Required(ErrorMessage = Error.RequiredFiled)]
		public string UserName { get; set; }


		[Required(ErrorMessage = Error.RequiredFiled)]
		[EmailAddress(ErrorMessage = Error.EmailAddress)]
		public string Email { get; set; }


		[Required(ErrorMessage = Error.RequiredFiled)]
		[Phone(ErrorMessage = Error.PhoneNumber)]
		public string PhoneNumber { get; set; }


		//[Required(ErrorMessage = Error.RequiredFiled)]
		//public DateTime DateOfBirth { get; set; }

	}
}
