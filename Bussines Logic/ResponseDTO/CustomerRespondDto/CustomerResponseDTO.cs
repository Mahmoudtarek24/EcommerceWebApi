using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussines_Logic.ResponseDTO
{
	public class CustomerResponseDTO
	{
		public int CustomerId { get; set; }	
		public string FirstName { get; set; }	
		public string lastName { get; set; }	
		public string Email { get; set; }	
		public string UserName { get; set; }	
		public string Password { get; set; }	
		public DateTime DateOfBirth { get; set; }
		public string PhoneNumber { get; set; }	
		public string message { get; set; }	


	}
}
