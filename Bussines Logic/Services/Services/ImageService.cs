
namespace Bussines_Logic.Services.Services
{
	public class ImageService : IImageService
	{
		private readonly IWebHostEnvironment webHostEnvironment;
		private readonly List<string> allowExtensions = new List<string>() { ".jpg", ".jpeg", ".png" };	
		private readonly int maxSize= 2097152;
		public ImageService(IWebHostEnvironment webHostEnvironment)
		{
			this.webHostEnvironment = webHostEnvironment;	
		}

		public async Task<UploadResult> UploadImage(IFormFile image,string folderPath )
		{
			var FolderPath = Path.Combine(webHostEnvironment.WebRootPath, folderPath);
			if(!Directory.Exists(FolderPath)) 
			{
				Directory.CreateDirectory(FolderPath);		
			}	

			var extenstion = Path.GetExtension(image.FileName);

			if (!allowExtensions.Contains(extenstion))
				return new UploadResult { ErrorMessage = Error.NotAllowedExtension, IsUploaded = false };
			if(image.Length> maxSize)
				return new UploadResult { ErrorMessage = Error.MaxSize, IsUploaded = false };


			var imageName = $"{Guid.NewGuid().ToString()}{extenstion}";
			var ImageFolderpath = Path.Combine($"{webHostEnvironment.WebRootPath}{folderPath}", imageName);

			using (var fileStream = new FileStream(ImageFolderpath, FileMode.Create, FileAccess.Write))
			{
				await image.CopyToAsync(fileStream);
			}

			return new UploadResult { ImageName = imageName, IsUploaded = true };
		}

		public void DeleteImage(string ImagePath) 
		{
			var oldImagePath = $"{webHostEnvironment.WebRootPath}/{ImagePath}";///  /image/propduct/fdgffbfggfr.jpg
		
			if (File.Exists(oldImagePath))
				File.Delete(oldImagePath);
		}


	}
}
