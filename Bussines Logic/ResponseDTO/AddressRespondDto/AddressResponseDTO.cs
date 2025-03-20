
namespace Bussines_Logic.ResponseDTO.AddressRespondDto
{
	public class AddressResponseDTO
	{
		public int Id { get; set; }
		public int CustomerId { get; set; }
		public string AddressLine1 { get; set; }
		public string AddressLine2 { get; set; }
		public string Area { get; set; }
		public string State { get; set; }
		public string PostalCode { get; set; }
		public string Governate { get; set; }
		public string Message { get; set; }
	}
}
