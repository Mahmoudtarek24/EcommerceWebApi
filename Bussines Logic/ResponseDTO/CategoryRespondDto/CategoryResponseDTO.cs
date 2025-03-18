
namespace Bussines_Logic.ResponseDTO.CategoryRespondDto
{
	public class CategoryResponseDTO 
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime CreateOn { get; set; } 
		public DateTime? LastUpdateOn { get; set; }
		public string Message { get; set; }	
	}
}
