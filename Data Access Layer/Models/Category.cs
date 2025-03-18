
namespace Data_Access_Layer.Models
{
	public class Category :BaseModal
	{
		public int CategoryId { get; set; }
		public string Name { get; set; } = null!;
		[MaxLength(300)]
		public string Description { get; set; } = null!;

		public List<Product> products { get; set; }

	}
}
