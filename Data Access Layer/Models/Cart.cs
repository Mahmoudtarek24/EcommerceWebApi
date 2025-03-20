

namespace Data_Access_Layer.Models
{
	public class Cart
	{
		public int cartId { get; set; }		
		public ICollection<CartItem> CartItems { get; set;} =new List<CartItem>();	


		public bool IsCheckedOut { get; set; } = false;
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }


		public int customerId { get; set; }	
		public Customer Customer { get; set; }

	}
}
