using Bussines_Logic.DTO.IDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussines_Logic.DTO
{
	public class LoginDTO : IDto
	{
		[Required(ErrorMessage = Error.RequiredFiled)]
		//[EmailAddress(ErrorMessage = Error.EmailAddress)]
		[Display(Name ="Email/UseName")]
		public string Email { get; set; }


		[Required(ErrorMessage = Error.RequiredFiled)]
		[MinLength(8, ErrorMessage = Error.MinLength)]
		public string Password { get; set; }

	}
}
