
namespace Bussines_Logic.DTO.PaymentDto
{
	public class buyingCartItemsDTO
	{
		public string PaymentMethod { get; set; } = null!;

		[Required(ErrorMessage = Error.RequiredFiled)]
		public int CustomerId { get; set; }


		[Required(ErrorMessage = Error.RequiredFiled)]
		public int BillingAddressId { get; set; }


		[Required(ErrorMessage = Error.RequiredFiled)]
		public int ShippingAddressId { get; set; }

		public int CartId { get; set; }	
	}
}
