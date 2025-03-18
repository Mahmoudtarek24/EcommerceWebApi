
namespace Bussines_Logic.Services.Services
{
	public class ApiResponse<T> 
	{
		public int StatusCode { get ; set; }
		public bool Success { get; set; }
		public T Data { get; set; }
		public List<string> Errors { get; set; }

		public ApiResponse()
		{
			Success = true;
			Errors = new List<string>();	
		}
		public ApiResponse(int statusCode, T data)
		{
			StatusCode=statusCode;
			Data = data;
			Success= true;
			Errors = new List<string>();
		}
		public ApiResponse(int statusCode, List<string> errors)
		{
			StatusCode = statusCode;
			Success = false;
			Errors = errors;
		}
		public ApiResponse(int statusCode, string error)
		{
			StatusCode = statusCode;
			Success = false;
			Errors = new List<string> { error };
		}
	}
}
