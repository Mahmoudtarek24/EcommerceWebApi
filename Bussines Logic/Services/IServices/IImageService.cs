
namespace Bussines_Logic.Services.IServices
{
	public interface IImageService
	{
		Task<UploadResult> UploadImage(IFormFile image, string folderPath);
		void DeleteImage(string ImagePath);
	}

}
