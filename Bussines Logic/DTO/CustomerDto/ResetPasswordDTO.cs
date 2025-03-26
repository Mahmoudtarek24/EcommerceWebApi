
namespace Bussines_Logic.DTO.CustomerDto
{
	public class ResetPasswordDTO
	{
		[Required,EmailAddress]
		public string Email { get; set; }	

	    public string Password { get; set; }
		[Compare("Password")]	
	    public string ConfirmPassword { get; set; }
		public string code { get; set; }	
	}
}
