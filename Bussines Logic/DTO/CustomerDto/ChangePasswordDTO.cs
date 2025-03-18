
namespace Bussines_Logic.DTO
{
	public class ChangePasswordDTO : IDto
	{
		[Required(ErrorMessage = Error.RequiredFiled)]
		public int CustomerId { get; set; }

		[Required(ErrorMessage = Error.RequiredFiled)]
		public string CurrentPassword { get; set; }

		[Required(ErrorMessage = Error.RequiredFiled)]
		[MinLength(8, ErrorMessage = Error.MinLength)]
		public string NewPassword { get; set; }


		[Required(ErrorMessage = Error.RequiredFiled)]
		[Compare("NewPassword", ErrorMessage = Error.MatchPassword)]
		public string ConfirmNewPassword { get; set; }
	}
}
