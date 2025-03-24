
namespace Data_Access_Layer.Models
{
	public class Order :BaseModal
	{

		public int OrderId { get; set; }

		[MaxLength(30)]
		public string OrderNumber { get; set; } = null!;
		public DateTime OrderDate { get; set; }

		public decimal TotalBaseAmount { get; set; }
		public decimal TotalDiscountAmount { get; set; }
		public decimal ShippingCost { get; set; }
		public decimal TotalAmount { get; set; }
		public OrderStatus OrderStatus { get; set; }


		public int BillingAddressId { get; set; }
		public Address BillingAddress { get; set; }


		public int ShippingAddressId { get; set; }
		public Address ShippingAddress { get; set; }


		public int CustomerId { get; set; }
		public Customer customer { get; set; }

		public ICollection<OrderItem> orderItems { get; set; }

		public Payment payment { get; set; }	

		public Cancellation cancellation { get; set; }		

	}
}
