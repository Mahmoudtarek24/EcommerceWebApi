
namespace Data_Access_Layer.Models
{
	public class Address
	{
		public int AddressId { get; set; }
		[MaxLength(200)]
		public string AddressLine1 { get; set; } = null!;
		[MaxLength(200)]
		public string? AddressLine2 { get; set; }

		public string Area { get; set; } = null!;
		
		public string State { get; set; } = null!;

		public string PostalCode { get; set; } = null!;

		public string Governorate { get; set; } = null!;

		public int CustomerId { get; set; }
		public Customer Customer { get; set; }

	}
}
