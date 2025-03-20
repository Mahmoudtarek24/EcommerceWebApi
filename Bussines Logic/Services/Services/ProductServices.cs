
namespace Bussines_Logic.Services.Services
{
	public class ProductServices<D> : IGenericService<ProductResponseDTO, D> where D : class
	{
		public IUnitOfWork unitOfWork { get; }
		private readonly string ProductImageFolderPath = "/Images/Product";
		private readonly string ProductRelatedImagesFolderPath = "/Images/Product/RelatedImages";
		private readonly IImageService imageService;
		public ProductServices(IUnitOfWork unitOfWork, IImageService imageService)
		{
			this.unitOfWork = unitOfWork;
			this.imageService = imageService;
		}

		public async Task<ApiResponse<ProductResponseDTO>> CreateAsync(D createDto)
		{
			if (createDto is ProductCreateDTO productDto)
			{
				var product = new Product();
				try
				{
					//validate category id 
					var category = await unitOfWork.CategoryRepository.GetByIdAsync(productDto.CategoryId);
					if (category is null)
						return new ApiResponse<ProductResponseDTO>(404, "Category Id For Product Not Found");

					await unitOfWork.CreateTarsaction();

					product.Description = productDto.Description;
					product.StockQuantity = productDto.StockQuantity;
					product.CategoryId = category.CategoryId;
					product.CreateOn = DateTime.Now;
					product.IsDeleted = false;
					product.Price = productDto.Price;
					product.ProductName = productDto.Name;
					product.DiscountPercentage = productDto.DiscountPercentage;

					product.IsAvailable = product.StockQuantity == 0 ?  false : true;

					var mainImageProductResult = await imageService.UploadImage(productDto.MainProductIamge, ProductImageFolderPath);

					if (!mainImageProductResult.IsUploaded)
						return new ApiResponse<ProductResponseDTO>(400, mainImageProductResult.ErrorMessage);

					product.mainProductImage = mainImageProductResult.ImageName;

					await unitOfWork.ProductRepository.Insert(product);
					await unitOfWork.Save();

					if (productDto.ProductImages != null)
					{

						foreach (var relatedProductImages in productDto.ProductImages)
						{
							var relatedImage = await imageService.UploadImage(relatedProductImages, ProductRelatedImagesFolderPath);
							if (!relatedImage.IsUploaded)
								return new ApiResponse<ProductResponseDTO>(400, relatedImage.ErrorMessage);

							var ImageProduct = new ProductImage()
							{
								ImageName = relatedImage.ImageName,
								ProductId = product.ProductId,
							};
							await unitOfWork.imageProducrRepository.Insert(ImageProduct);
							await unitOfWork.Save();
						}
					}
					await unitOfWork.Commit();

					var CreateRespond = new ProductResponseDTO()
					{
						Id = product.ProductId,
						Description = product.Description,
						DiscountPercentage = product.DiscountPercentage,
						StockQuantity = product.StockQuantity,
						CategoryId = product.CategoryId,
						IsAvailable = product.IsAvailable,
						CreatedOn = product.CreateOn,
						Name = product.ProductName,
						IsDeleted = product.IsDeleted,
						Price = product.Price,
						ImageUrl = $"{ProductImageFolderPath}/{product.mainProductImage}",
						message = "Created Product",
					};
					var RelatedImages = await unitOfWork.imageProducrRepository.GetAllEntitiesAsync(e => e.ProductId == product.ProductId);
					if (RelatedImages != null)
					{
						foreach (var image in RelatedImages)
							CreateRespond.RelatedImages.Add($"{ProductRelatedImagesFolderPath}/{image.ImageName}");
					}
					return new ApiResponse<ProductResponseDTO>(200, CreateRespond);
				}
				catch (Exception ex)
				{
					imageService.DeleteImage($"{ProductImageFolderPath}/{product.mainProductImage}");
					if (product.mainProductImage != null)
					{
						foreach (var image in product.productImages)
						{
							imageService.DeleteImage($"{ProductRelatedImagesFolderPath}/{image.ImageName}");
						}
					}
					await unitOfWork.RollBack();
					return new ApiResponse<ProductResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
				}
			}
			else
			{
				return new ApiResponse<ProductResponseDTO>(400, "Invalid DTO type for Address registration.");
			}
		}

		public async Task<ApiResponse<ProductResponseDTO>> DeleteAsync(int id)
		{
			try
			{
				var product = await unitOfWork.ProductRepository.GetByIdAsync(id);
				if (product is null)
					return new ApiResponse<ProductResponseDTO>(404, "Product Id not valid ");

				await unitOfWork.CreateTarsaction();

				product.IsDeleted = !product.IsDeleted;
				product.LastUpdateOn = DateTime.Now;
				product.IsAvailable = !product.IsAvailable;

				await unitOfWork.Save();
				await unitOfWork.Commit();

				var result = product.IsDeleted ? "Deleted" : "Retrived";

				var respond = new ProductResponseDTO()
				{
					message = $"Product With Id {product.ProductId} Is {result} Sussefully",
					LastUpdateOn = product.LastUpdateOn,
				};
				return new ApiResponse<ProductResponseDTO>(200, respond);
			}
			catch (Exception ex)
			{
				await unitOfWork.RollBack();
				return new ApiResponse<ProductResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}
		}

		public async Task<ApiResponse<ProductResponseDTO>> GetByIdAsync(int id)
		{
			try
			{
				string[] Includes = { "Category", "productImages" };
				var product = await unitOfWork.ProductRepository.GetEntityAsync(e => e.ProductId == id, Includes, true);
				if (product is null)
					return new ApiResponse<ProductResponseDTO>(404, "Product not Found");

				var respond = new ProductResponseDTO()
				{
					Description = product.Description,
					CategoryId = product.CategoryId,
					CreatedOn = product.CreateOn,
					LastUpdateOn = product.LastUpdateOn,
					DiscountPercentage = product.DiscountPercentage,
					StockQuantity = product.StockQuantity,
					Id = product.ProductId,
					Price = product.Price,
					IsDeleted = product.IsDeleted,
					IsAvailable = product.IsAvailable,
					Name = product.ProductName,
					ImageUrl = $"{ProductImageFolderPath}/{product.mainProductImage}",
				};
				if (product.productImages != null)
				{
					foreach (var image in product.productImages)
						respond.RelatedImages.Add($"{ProductRelatedImagesFolderPath}/{image.ImageName}");
				}

				return new ApiResponse<ProductResponseDTO>(200, respond);
			}
			catch (Exception ex)
			{
				return new ApiResponse<ProductResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}

		}

		public async Task<ApiResponse<ProductResponseDTO>> UpdateAsync(D upateDto)
		{
			if (upateDto is ProductUpdateDTO productDto)
			{
				string[] Includes = { "Category", "productImages" };
				var product = await unitOfWork.ProductRepository.GetEntityAsync(e => e.ProductId == productDto.ProductId, Includes);
				if (product is null)
					return new ApiResponse<ProductResponseDTO>(404, $"Product with Id {productDto.ProductId} not found");

				var category = await unitOfWork.CategoryRepository.GetByIdAsync(productDto.CategoryId);
				if (category is null)
					return new ApiResponse<ProductResponseDTO>(404, $"Category with Id {category.CategoryId} not Valid");

				try
				{
					await unitOfWork.CreateTarsaction();

					product.LastUpdateOn = DateTime.Now;
					product.DiscountPercentage = productDto.DiscountPercentage;
					product.Price = productDto.Price;
					product.CategoryId = productDto.CategoryId;
					product.Description = productDto.Description;
					product.StockQuantity = productDto.StockQuantity;
					product.ProductName = productDto.Name;
					product.IsAvailable = product.StockQuantity == 0 ? false : true;
					//Delete Main Image
					imageService.DeleteImage($"{ProductImageFolderPath}/{product.mainProductImage}");

					var mainImageProductResult = await imageService.UploadImage(productDto.MainProductIamge, ProductImageFolderPath);

					if (!mainImageProductResult.IsUploaded)
						return new ApiResponse<ProductResponseDTO>(400, mainImageProductResult.ErrorMessage);

					product.mainProductImage = mainImageProductResult.ImageName;

					await unitOfWork.Save();


					foreach (var image in product.productImages)
					{
						imageService.DeleteImage($"{ProductRelatedImagesFolderPath}/{image.ImageName}");
					}

					foreach (var relatedProductImages in productDto.ProductImages)
					{
						var relatedImage = await imageService.UploadImage(relatedProductImages, ProductRelatedImagesFolderPath);
						if (!relatedImage.IsUploaded)
							return new ApiResponse<ProductResponseDTO>(400, relatedImage.ErrorMessage);

						var ImageProduct = new ProductImage()
						{
							ImageName = relatedImage.ImageName,
							ProductId = product.ProductId,
						};
						await unitOfWork.imageProducrRepository.Insert(ImageProduct);
						await unitOfWork.Save();
					}

					await unitOfWork.Commit();

					var UpdetRespond = new ProductResponseDTO()
					{
						Id = product.ProductId,
						Description = product.Description,
						DiscountPercentage = product.DiscountPercentage,
						StockQuantity = product.StockQuantity,
						CategoryId = product.CategoryId,
						IsAvailable = product.IsAvailable,
						CreatedOn = product.CreateOn,
						Name = product.ProductName,
						IsDeleted = product.IsDeleted,
						Price = product.Price,
						ImageUrl = $"{ProductImageFolderPath}/{product.mainProductImage}",
						message = "Update Product",
					};
					var RelatedImages = await unitOfWork.imageProducrRepository.GetAllEntitiesAsync(e => e.ProductId == product.ProductId);
					if (RelatedImages != null)
					{
						foreach (var image in RelatedImages)
							UpdetRespond.RelatedImages.Add($"{ProductRelatedImagesFolderPath}/{image.ImageName}");
					}
					return new ApiResponse<ProductResponseDTO>(200, UpdetRespond);

				}
				catch (Exception ex)
				{
					imageService.DeleteImage($"{ProductImageFolderPath}/{product.mainProductImage}");
					if (product.mainProductImage != null)
					{
						foreach (var image in product.productImages)
						{
							imageService.DeleteImage($"{ProductRelatedImagesFolderPath}/{image.ImageName}");
						}
					}
					await unitOfWork.RollBack();
					return new ApiResponse<ProductResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
				}
			}
			else
			{
				return new ApiResponse<ProductResponseDTO>(400, "Invalid DTO type for Address registration.");
			}
		}
		public async Task<ApiResponse<List<ProductResponseDTO>>> GetAllProductsByCategoryAsync(int categoryId)
		{
			try
			{
				var category = await unitOfWork.CategoryRepository.GetByIdAsync(categoryId);
				if (category is null)
					return new ApiResponse<List<ProductResponseDTO>>(404, $"no category for Id {categoryId}");

				string[] Includes = { "Category", "productImages" };
				var products = await unitOfWork.ProductRepository.GetAllEntitiesAsync(e => e.CategoryId == categoryId, Includes, true);

				if (products.Count() == 0)
					return new ApiResponse<List<ProductResponseDTO>>(404, $"No Product for Category Id{categoryId}");


				List<ProductResponseDTO> productResponses = new List<ProductResponseDTO>();

				foreach (var product in products)
				{
					var respond = new ProductResponseDTO()
					{
						Description = product.Description,
						CategoryId = product.CategoryId,
						CreatedOn = product.CreateOn,
						LastUpdateOn = product.LastUpdateOn,
						DiscountPercentage = product.DiscountPercentage,
						StockQuantity = product.StockQuantity,
						Id = product.ProductId,
						Price = product.Price,
						IsDeleted = product.IsDeleted,
						IsAvailable = product.IsAvailable,
						Name = product.ProductName,
						ImageUrl = $"{ProductImageFolderPath}/{product.mainProductImage}",
					};
					if (product.productImages != null)
					{
						foreach (var image in product.productImages)
							respond.RelatedImages.Add($"{ProductRelatedImagesFolderPath}/{image.ImageName}");
					}
					productResponses.Add(respond);
				}
				return new ApiResponse<List<ProductResponseDTO>>(200, productResponses);
			}
			catch (Exception ex)
			{
				return new ApiResponse<List<ProductResponseDTO>>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");

			}
		}
		public async Task<ApiResponse<ConfirmationResponseDTO>> UpdateProductStatusAsync(ProductStatusUpdateDTO productStatusUpdateDTO)
		{
			try
			{
				var product = await unitOfWork.ProductRepository.GetEntityAsync(e => e.ProductId == productStatusUpdateDTO.ProductId);

				if (product is null)
					return new ApiResponse<ConfirmationResponseDTO>(404, $"Product with Id {productStatusUpdateDTO.ProductId} not found");

				product = await unitOfWork.ProductRepository.GetEntityAsync(e => e.ProductId == productStatusUpdateDTO.ProductId && !e.IsDeleted);

				if (product is null)
					return new ApiResponse<ConfirmationResponseDTO>(404, $"Product with Id {productStatusUpdateDTO.ProductId} was Deleted");


				ConfirmationResponseDTO confirmationResponse = new ConfirmationResponseDTO();
				if (product.StockQuantity == 0)
				{
					product.IsAvailable = false;
					confirmationResponse.Message = Error.stockQuntity;
					return new ApiResponse<ConfirmationResponseDTO>(200,confirmationResponse);
				}

				await unitOfWork.CreateTarsaction();

				product.LastUpdateOn = DateTime.Now;
				product.IsAvailable = productStatusUpdateDTO.IsAvailable;

				await unitOfWork.Save();
				await unitOfWork.Commit();
				confirmationResponse.Message = $"Product with Id {productStatusUpdateDTO.ProductId} Status Updated successfully.";

				return new ApiResponse<ConfirmationResponseDTO>(200, confirmationResponse);
			}
			catch (Exception ex)
			{
				await unitOfWork.RollBack();
				return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}
		}
	}
}

