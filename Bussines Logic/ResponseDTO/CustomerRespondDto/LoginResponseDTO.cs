using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussines_Logic.ResponseDTO
{
	public class LoginResponseDTO
	{
		public int CustomerId { get; set; }	
		public string CustomerName { get; set; }
		public string Message { get; set; }
	}
}
