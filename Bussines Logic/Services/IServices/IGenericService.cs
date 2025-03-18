
namespace Bussines_Logic.Services.IServices
{
	public interface IGenericService<T, D> where T : class where D : class
	{
		IUnitOfWork unitOfWork { get; }
		Task<ApiResponse<T>> CreateAsync(D createDto);
		//	Task<ApiResponse<T>> LoinAsync(D updateDto);	
		Task<ApiResponse<T>> UpdateAsync(D upateDto);
		Task<ApiResponse<T>> GetByIdAsync(int id);
		Task<ApiResponse<T>> DeleteAsync(int id);
	}
}
