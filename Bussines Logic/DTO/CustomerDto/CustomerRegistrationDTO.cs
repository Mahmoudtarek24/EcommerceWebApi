﻿
namespace Bussines_Logic.DTO
{
	public class CustomerRegistrationDTO : IDto
	{
		[Required(ErrorMessage = Error.RequiredFiled)]
		[EmailAddress(ErrorMessage =Error.EmailAddress)]
		public string Email { get; set; }


		[Required(ErrorMessage = Error.RequiredFiled)]
		public string UserName { get; set; }
		

		[Required(ErrorMessage = Error.RequiredFiled)]
		[MinLength(2, ErrorMessage = Error.MinLength)]
		[MaxLength(50, ErrorMessage = Error.MaxLength)]
		public string FirstName { get; set; }


		[Required(ErrorMessage = Error.RequiredFiled)]
		[MinLength(2, ErrorMessage = Error.MinLength)]
		[MaxLength(50, ErrorMessage = Error.MaxLength)]
		public string LastName { get; set; }


		[Required(ErrorMessage = Error.RequiredFiled)]
		[MinLength(8, ErrorMessage = Error.MinLength)]
		public string Password { get; set; }


		[Required(ErrorMessage = Error.RequiredFiled)]
		[Compare("Password",ErrorMessage =Error.ConfirmPasswordNotMatch)]
		public string ConfirmPassword { get; set; }


		[Required(ErrorMessage = Error.RequiredFiled)]
		[Phone(ErrorMessage =Error.PhoneNumber)]
		public string PhoneNumber { get; set; }

		public IFormFile? ImageData { get; set; }

		[Required(ErrorMessage = Error.RequiredFiled)]
		[AgeRange(100)]
		public DateTime DateOfBirth { get; set; }
	}
}
