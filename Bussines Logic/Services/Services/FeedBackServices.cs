
using Bussines_Logic.DTO.FeedBackDto;
using Data_Access_Layer.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bussines_Logic.Services.Services
{
	public class FeedBackServices
	{
		private readonly IUnitOfWork unitOfWork;
		private readonly EcommerceDbContext context;
		public FeedBackServices(IUnitOfWork unitOfWork ,EcommerceDbContext context)
		{
			this.unitOfWork = unitOfWork;
			this.context = context;	
		}

		public async Task<ApiResponse<FeedbackResponseDTO>> SubmitFeedbackAsync(FeedBackCreateDTO feedBackCreateDTO)
		{
			try {
				await unitOfWork.CreateTarsaction();
				 var customer=await unitOfWork.CustomerRepository.GetByIdAsync(feedBackCreateDTO.CustomerId);
				if (customer is null)
					return new ApiResponse<FeedbackResponseDTO>(404, $"Customer With Id: {feedBackCreateDTO.CustomerId} Not Found");
				
				var product=await unitOfWork.ProductRepository.GetByIdAsync(feedBackCreateDTO.ProductId);
				if (product is null)
					return new ApiResponse<FeedbackResponseDTO>(404, $"Product With Id: {feedBackCreateDTO.ProductId} Not Found");

				// Verify order item exists and belongs to customer and product (Order must be delivered)
				string[] Includes = { "order" };
				var orderItem= await unitOfWork.OrderItemRepository.GetEntityAsync(e=>e.ProductId==feedBackCreateDTO.ProductId&&e.order.CustomerId==feedBackCreateDTO.CustomerId&&e.order.OrderStatus==OrderStatus.Delivered, Includes,true);

				if (orderItem is null)
					return new ApiResponse<FeedbackResponseDTO>(400, "Customer must have purchased the product.");
				if(await unitOfWork.FeedbackRepository.GetAnyEntityAsync(fed => fed.CustomerId == feedBackCreateDTO.CustomerId && fed.ProductId == feedBackCreateDTO.ProductId))
					return new ApiResponse<FeedbackResponseDTO>(400, "Feedback for this product and order item already exists.");

				var feedBack = new Feedback()
				{
					CustomerId= feedBackCreateDTO.CustomerId,	
					CreatedAt=DateTime.Now,
					Comment= feedBackCreateDTO.Comment,	
					ProductId=feedBackCreateDTO.ProductId,
					Rating= feedBackCreateDTO.Reting,
				};
				
				await unitOfWork.FeedbackRepository.Insert(feedBack);
				await unitOfWork.Save();
				await unitOfWork.Commit();

				var respond = MapFeedBackToDTO(feedBack.FeedbackId);
				unitOfWork.Dispose();

				return new ApiResponse<FeedbackResponseDTO>(200,respond);		

			}
			catch (Exception ex) {
				await unitOfWork.RollBack();
				return new ApiResponse<FeedbackResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}	
		}
		public async Task<ApiResponse<ProductFeedbackResponseDTO>> GetFeedbackForProduct(int productId)
		{
			try
			{
				var product = await unitOfWork.ProductRepository.GetByIdAsync(productId);
				if (product is null)
					return new ApiResponse<ProductFeedbackResponseDTO>(404, $"Product with Id {productId} Not Found");

				string[] Includes = { "Customer", "Customer.applicationUser" };
				var feedbacks = await unitOfWork.FeedbackRepository.GetAllEntitiesAsync(e => e.ProductId == productId, Includes);

				double averageRating = 0;
				List<CustomerFeedback> customerFeedbacks = new List<CustomerFeedback>();
				if (feedbacks.Any())
				{
					averageRating=feedbacks.Average(e => e.Rating);		
					foreach(var feedback in feedbacks) {
						CustomerFeedback customerFeedback = new CustomerFeedback()
						{
							CustomerId= feedback.CustomerId,	
							CustomerName=$"{feedback.Customer.applicationUser.FirstName} {feedback.Customer.applicationUser.LastName}",
							Comment= feedback.Comment,	
							CreatedAt= feedback.CreatedAt,
							Id=feedback.FeedbackId,
							Rating=feedback.Rating,
							UpdatedAt= feedback.UpdatedAt,	
						};
						customerFeedbacks.Add(customerFeedback);	
					}	
				}
				var productFeedbackResponse = new ProductFeedbackResponseDTO
				{
					ProductId = product.ProductId,
					ProductName = product.ProductName,
					AverageRating = Math.Round(averageRating, 2),
					Feedbacks = customerFeedbacks
				};
				return new ApiResponse<ProductFeedbackResponseDTO>(200, productFeedbackResponse);

			}
			catch (Exception ex)
			{
				return new ApiResponse<ProductFeedbackResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}

		}
		public async Task<ApiResponse<List<FeedbackResponseDTO>>> GetAllFeedbackAsync()
		{
			try
			{
				string[] Includes = { "Customer", "Customer.applicationUser", "Product" };

				var feedbacks = await unitOfWork.FeedbackRepository.GetAllEntitiesAsync(null, Includes);

				List<FeedbackResponseDTO> feedbackResponses = new List<FeedbackResponseDTO>();
				foreach (var feedback in feedbacks)
				{
					FeedbackResponseDTO feedbackResponse = MapFeedBackToDTO(feedback.FeedbackId);
					feedbackResponses.Add(feedbackResponse);
				}
				return new ApiResponse<List<FeedbackResponseDTO>>(200, feedbackResponses);

			}
			catch (Exception ex)
			{
				return new ApiResponse<List<FeedbackResponseDTO>>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}
		}

		public async Task<ApiResponse<FeedbackResponseDTO>> UpdateFeedbackAsync(FeedbackUpdateDTO feedbackUpdateDTO)
		{
			try {
				await unitOfWork.CreateTarsaction();
				string[] Includes = { "Customer", "Customer.applicationUser", "Product" };

				var feedback = await unitOfWork.FeedbackRepository.GetEntityAsync(e=>e.FeedbackId==feedbackUpdateDTO.FeedbackId, Includes);
				if (feedback is null)
					return new ApiResponse<FeedbackResponseDTO>(404, $"FeedBack With Id {feedbackUpdateDTO.FeedbackId} not Found");

				feedback.Comment= feedbackUpdateDTO.Comment;	
				feedback.Rating= feedbackUpdateDTO.Rating;	
			    feedback.UpdatedAt=DateTime.Now;

				await unitOfWork.Save();
				await unitOfWork.Commit();

				FeedbackResponseDTO feedbackResponse = MapFeedBackToDTO(feedback.FeedbackId);

				return new ApiResponse<FeedbackResponseDTO>(200,feedbackResponse);	
			}
			catch(Exception ex) {
				return new ApiResponse<FeedbackResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}
		}
		public async Task<ApiResponse<ConfirmationResponseDTO>> DeleteFeedbackAsync(FeedbackDeleteDTO feedbackDeleteDTO)
		{
			try {
				await unitOfWork.CreateTarsaction();
				string[] Includes = { "Customer" };
				var feedback = await unitOfWork.FeedbackRepository.GetEntityAsync(e => e.FeedbackId == feedbackDeleteDTO.FeedbackId, Includes);
				if (feedback is null)
					return new ApiResponse<ConfirmationResponseDTO>(404, $"FeedBack With Id {feedbackDeleteDTO.FeedbackId} not Found");
				
				if (feedback.CustomerId != feedbackDeleteDTO.CustomerId)
				{
					return new ApiResponse<ConfirmationResponseDTO>(401, "You are not authorized to delete this feedback.");
				}

				await unitOfWork.FeedbackRepository.Delete(feedback);
				await unitOfWork.Save();
				await unitOfWork.Commit();

				var confirmation = new ConfirmationResponseDTO
				{
					Message = $"Feedback with Id {feedbackDeleteDTO.FeedbackId} deleted successfully."
				};
				return new ApiResponse<ConfirmationResponseDTO>(200, confirmation);


			}
			catch (Exception ex) {
				await unitOfWork.RollBack();
				return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}
		}
		private FeedbackResponseDTO MapFeedBackToDTO(int feedBackId)
		{
			var feedBack = unitOfWork.FeedbackRepository.FeedBackInformation(e => e.FeedbackId == feedBackId);

			var feedBackRespond = new FeedbackResponseDTO()
			{
				Comment = feedBack.Comment,
				CreatedAt = feedBack.CreatedAt,
				CustomerId = feedBack.CustomerId,
				ProductId = feedBack.ProductId,
				Rating = feedBack.Rating,
				UpdatedAt = feedBack.UpdatedAt,
				Id = feedBack.FeedbackId,
				CustomerName = $"{feedBack.Customer.applicationUser.FirstName} {feedBack.Customer.applicationUser.LastName}",
				ProductName = feedBack.Product.ProductName
			};
			return feedBackRespond;
		}
		
	}
}
