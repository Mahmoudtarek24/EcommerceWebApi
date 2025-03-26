using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussines_Logic.DTO.CustomerDto
{
	public class ForgotPasswordDto
	{

		[Required(ErrorMessage =Error.RequiredFiled)]
		[EmailAddress]
		public string Email { get; set; } = null!;
	}
}
