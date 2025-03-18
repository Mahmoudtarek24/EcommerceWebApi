
namespace Data_Access_Layer.Models
{
	public class BaseModal
	{
		public bool IsDeleted { get; set; }	
		public DateTime CreateOn { get; set; }= DateTime.Now;	
		public DateTime? LastUpdateOn { get; set; }

	}
}
