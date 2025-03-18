
using Bussines_Logic.DTO.IDTO;

namespace Bussines_Logic.Services.Services
{
	public class CategoryServices<D> : IGenericService<CategoryResponseDTO, D> where D : class
	{
		public IUnitOfWork unitOfWork { get; }
		public CategoryServices(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
		}

		public async Task<ApiResponse<CategoryResponseDTO>> CreateAsync(D createDto)
		{
			if (createDto is CategoryCreateDTO categoryDto)
			{
				try
				{
					var ctegory = await unitOfWork.CategoryRepository.GetEntityAsync(e => e.Name == categoryDto.Name);
					if(ctegory is not null)
						return new ApiResponse<CategoryResponseDTO>(400,$"{categoryDto.Name} Category is Created Before.");

					await unitOfWork.CreateTarsaction();

					var category = new Category()
					{
						CreateOn = DateTime.Now,
						Description = categoryDto.Description,
						IsDeleted = false,
						Name = categoryDto.Name,
					};
					await unitOfWork.CategoryRepository.Insert(category);
					await unitOfWork.Save();
					await unitOfWork.Commit();

					var createRespond = new CategoryResponseDTO()
					{
						CreateOn = category.CreateOn,
						Description = category.Description,
						Id = category.CategoryId,
						Name = category.Name,
						IsDeleted = category.IsDeleted,
						LastUpdateOn = category.LastUpdateOn,
						Message = "Created Successfully"
					};
					return new ApiResponse<CategoryResponseDTO>(200, createRespond);

				}
				catch (Exception ex)
				{
					await unitOfWork.RollBack();
					return new ApiResponse<CategoryResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
				}
			}
			else
			{
				return new ApiResponse<CategoryResponseDTO>(400, "Invalid DTO type for Address registration.");
			}
		}

		public async Task<ApiResponse<CategoryResponseDTO>> DeleteAsync(int id)
		{
			try
			{
				var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);
				if (category is null)
					return new ApiResponse<CategoryResponseDTO>(404, "Category not Found");

				await unitOfWork.CreateTarsaction();
				await unitOfWork.CategoryRepository.Delete(category);
				await unitOfWork.Save();
				await unitOfWork.Commit();

				var respond = new CategoryResponseDTO()
				{
					Message = $"Category with Id {id}  Deleted successfully."
				};
				return new ApiResponse<CategoryResponseDTO>(200, respond);

			}
			catch (Exception ex)
			{
				await unitOfWork.RollBack();
				return new ApiResponse<CategoryResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}
		}

		public async Task<ApiResponse<CategoryResponseDTO>> GetByIdAsync(int id)
		{
			try
			{
				var category = await unitOfWork.CategoryRepository.GetEntityAsync(e => e.CategoryId == id);
				if (category is null)
					return new ApiResponse<CategoryResponseDTO>(404, "Category not found.");

				var Respond = new CategoryResponseDTO()
				{
					CreateOn = category.CreateOn,
					Description = category.Description,
					Id = category.CategoryId,
					Name = category.Name,
					IsDeleted = category.IsDeleted,
					LastUpdateOn = category.LastUpdateOn,
					Message = "category Data"
				};
				return new ApiResponse<CategoryResponseDTO>(200, Respond);
			}
			catch (Exception ex)
			{
				return new ApiResponse<CategoryResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}
		}

		public async Task<ApiResponse<CategoryResponseDTO>> UpdateAsync(D upateDto)
		{
			if (upateDto is CategoryUpdateDTO updateDTO)
			{
				try
				{
					var category = await unitOfWork.CategoryRepository.GetEntityAsync(e => e.CategoryId == updateDTO.CategoryId);
					if (category is null)
						return new ApiResponse<CategoryResponseDTO>(404, "Category not found.");

					await unitOfWork.CreateTarsaction();
					category.LastUpdateOn = DateTime.Now;
					category.Description = updateDTO.Description;
					category.Name = updateDTO.Name;

					await unitOfWork.Save();
					await unitOfWork.Commit();

					var respond = new CategoryResponseDTO
					{
						CreateOn = category.CreateOn,
						Description = category.Description,
						Id = category.CategoryId,
						Name = category.Name,
						IsDeleted = category.IsDeleted,
						LastUpdateOn = category.LastUpdateOn,
						Message = "Update Successfully"
					};
					return new ApiResponse<CategoryResponseDTO>(200, respond);
				}
				catch (Exception ex)
				{
					await unitOfWork.RollBack();
					return new ApiResponse<CategoryResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
				}
			}
			else
			{
				return new ApiResponse<CategoryResponseDTO>(400, "Invalid DTO type for Address registration.");
			}
		}



		public async Task<ApiResponse<List<CategoryResponseDTO>>> GetAllCategory()
		{
			try
			{
				var Categorys = await unitOfWork.CategoryRepository.GetAllEntitiesAsync(null, null, true);

				var RespondList = new List<CategoryResponseDTO>();
				foreach (var Category in Categorys)
				{
					var category = new CategoryResponseDTO()
					{
						Description = Category.Description,
						CreateOn = Category.CreateOn,
						IsDeleted = Category.IsDeleted,
						Id = Category.CategoryId,
						Name = Category.Name,
						LastUpdateOn = Category.LastUpdateOn,

					};
					RespondList.Add(category);
				}
				return new ApiResponse<List<CategoryResponseDTO>>(200, RespondList);

			}
			catch (Exception ex)
			{
				return new ApiResponse<List<CategoryResponseDTO>>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}
		}
	}
}
