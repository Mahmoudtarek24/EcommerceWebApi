﻿
namespace Data_Access_Layer.Models
{
	public class Customer 
	{
		public int Id { get; set; }	
		public DateTime DateOfBirth { get; set; }
		[MaxLength(450)]
		public string UserId { get; set; } = null!;
		public ApplicationUser applicationUser { get; set; }

		public ICollection<Address> Addresses { get; set; }
	}
}
