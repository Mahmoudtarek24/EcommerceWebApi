
namespace Bussines_Logic.Services.Services
{
	public class ProductServices<D> : IGenericService<CustomerResponseDTO, D> where D : class
	{
		public IUnitOfWork unitOfWork { get;}
		private readonly IWebHostEnvironment webHostEnvironment;
		public ProductServices(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
		}

		public Task<ApiResponse<CustomerResponseDTO>> CreateAsync(D createDto)
		{
			throw new NotImplementedException();
		}

		public Task<ApiResponse<CustomerResponseDTO>> DeleteAsync(int id)
		{
			throw new NotImplementedException();
		}

		public Task<ApiResponse<CustomerResponseDTO>> GetByIdAsync(int id)
		{
			throw new NotImplementedException();
		}

		public Task<ApiResponse<CustomerResponseDTO>> UpdateAsync(D upateDto)
		{
			throw new NotImplementedException();
		}
	}
}
