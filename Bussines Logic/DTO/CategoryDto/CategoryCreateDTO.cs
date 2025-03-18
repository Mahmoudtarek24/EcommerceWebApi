
namespace Bussines_Logic.DTO.CategoryDto
{
	public class CategoryCreateDTO :IDto
	{
		[Required(ErrorMessage = Error.RequiredFiled)]
		[Display(Name ="Category Name"),StringLength(100, MinimumLength = 3, ErrorMessage = Error.StringLent)]
		public string Name { get; set; }
		[StringLength(500, ErrorMessage = Error.MaxLength)]
		[Required(ErrorMessage = Error.RequiredFiled)]
		public string Description { get; set; }
	}
}
