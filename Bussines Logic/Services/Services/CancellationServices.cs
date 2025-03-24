
namespace Bussines_Logic.Services.Services
{
	public class CancellationServices
	{
		private const decimal DefaultCancellationCharge = 0.0m;
		private readonly IEmailSender emailSender;
		private readonly IWebHostEnvironment webHostEnvironment;
		public IUnitOfWork unitOfWork { get; }
		public CancellationServices(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
		{
			this.unitOfWork = unitOfWork;
			this.webHostEnvironment = webHostEnvironment;	
			this.emailSender = emailSender;	
		}

		//If Product arrive Cant access this action  //this for customer to make request
		public async Task<ApiResponse<CancellationRespondDTO>> RequestCancellationAsync(CancellationRequestDTO cancellationRequest)
		{
			try
			{
				await unitOfWork.CreateTarsaction();
				var order = await unitOfWork.OrderRepository.CancellationOrders(e => e.OrderId == cancellationRequest.OrderId && e.customer.Id == cancellationRequest.CustomerId);

				if (order == null || order.customer == null)
					return new ApiResponse<CancellationRespondDTO>(404, "Order or Customer Not Found");

				if (order.OrderStatus == OrderStatus.Delivered)
					return new ApiResponse<CancellationRespondDTO>(400, "Order is Delivered Can not cancellation it");

				var existingCancellation = await unitOfWork.cancellationRepository.GetEntityAsync(c => c.OrderId == cancellationRequest.OrderId);
				if (existingCancellation != null)
					return new ApiResponse<CancellationRespondDTO>(400, "A cancellation request for this order already exists.");

				if (order.OrderStatus == OrderStatus.Pending || order.OrderStatus == OrderStatus.Processing)
				{

					Cancellation cancellation = new Cancellation()
					{
						OrderId = cancellationRequest.OrderId,
						Reason = cancellationRequest.Reason,
						RequestedAt = DateTime.Now,
						cancellationStatus = CancellationStatus.Pending,
						CancellationCharges = DefaultCancellationCharge,
						OrderAmount = order.TotalAmount,

					};
					await unitOfWork.cancellationRepository.Insert(cancellation);
					await unitOfWork.Save();
					await unitOfWork.Commit();

					var respond = MapCancellationToDTO(cancellation);
					return new ApiResponse<CancellationRespondDTO>(200, respond);
				}
				else
				{
					return new ApiResponse<CancellationRespondDTO>(400, "Order is not eligible for cancellation.");
				}
			}
			catch (Exception ex)
			{
				await unitOfWork.RollBack();
				return new ApiResponse<CancellationRespondDTO>(500, $"An unexpected error occurred: {ex.Message}");
			}
		}
		public async Task<ApiResponse<List<CancellationRespondDTO>>> GetAllCancellationsAsync()
		{
			try
			{
				var cancellations = await unitOfWork.cancellationRepository.GetAllEntitiesAsync(null, null, true);
				if (cancellations.Count() == 0)
					return new ApiResponse<List<CancellationRespondDTO>>(200, "No Cancellation occure yet");

				var CancellationList = new List<CancellationRespondDTO>();

				foreach (var cancellation in cancellations)
				{
					CancellationRespondDTO cancellationRespond = MapCancellationToDTO(cancellation);
					CancellationList.Add(cancellationRespond);
				}
				return new ApiResponse<List<CancellationRespondDTO>>(200, CancellationList);

			}
			catch (Exception ex)
			{
				return new ApiResponse<List<CancellationRespondDTO>>(500, $"An unexpected error occurred: {ex.Message}");
			}
		}
		public async Task<ApiResponse<CancellationRespondDTO>> GetCancellationById(int id)
		{
			try
			{
				var cancellation = await unitOfWork.cancellationRepository.GetByIdAsync(id);
				if (cancellation is null)
					return new ApiResponse<CancellationRespondDTO>(404, $"cancellations With Id : {id} Not Found");

				var respond = MapCancellationToDTO(cancellation);
				return new ApiResponse<CancellationRespondDTO>(200, respond);
			}
			catch (Exception ex)
			{
				return new ApiResponse<CancellationRespondDTO>(500, $"An unexpected error occurred: {ex.Message}");
			}
		}
		//this for admin to process request 
		public async Task<ApiResponse<ConfirmationResponseDTO>> UpdateCancellationStatusAsync(CancellationStatusUpdateDTO statusUpdateDTO)
		{
			try
			{
				await unitOfWork.CreateTarsaction();
				var cancellation = await unitOfWork.cancellationRepository.CancellationOrders(e => e.CancellationId == statusUpdateDTO.CancellationId);
				if (cancellation is null)
					return new ApiResponse<ConfirmationResponseDTO>(404, "Cancellation request not found.");

				if (cancellation.cancellationStatus != CancellationStatus.Pending)
					return new ApiResponse<ConfirmationResponseDTO>(400, "Only pending cancellation requests can be updated.");

				cancellation.cancellationStatus = statusUpdateDTO.Status;
				cancellation.ProcessedAt = DateTime.Now;
				cancellation.ProcessedBy = statusUpdateDTO.ProcessedBy;
				cancellation.Remarks = statusUpdateDTO.Remarks;

				if (statusUpdateDTO.Status == CancellationStatus.Approved)
				{
					cancellation.Order.OrderStatus = OrderStatus.Canceled;
					cancellation.CancellationCharges=statusUpdateDTO.CancellationCharges;
					await unitOfWork.Save();
					
					foreach (var orderItem in cancellation.Order.orderItems)
					{
						orderItem.product.StockQuantity += orderItem.Quantity;
					}
					await unitOfWork.Save();

					await unitOfWork.Commit();
					await NotifyCancellationAcceptedAsync(cancellation);
				}
				if(statusUpdateDTO.Status == CancellationStatus.Rejected) {

					await NotifyCancellationRejectionAsync(cancellation);
				}

				var confirmation = new ConfirmationResponseDTO
				{
					Message = $"Cancellation request with ID {cancellation.CancellationId} has been {cancellation.cancellationStatus}."
				};
				return new ApiResponse<ConfirmationResponseDTO>(200, confirmation);
			}
			catch (Exception ex) {
				await unitOfWork.RollBack();
				return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occurred: {ex.Message}");
			}

		}

		private CancellationRespondDTO MapCancellationToDTO(Cancellation cancellation)
		{
			var CancellationResponse = new CancellationRespondDTO()
			{
				Id = cancellation.CancellationId,
				OrderId = cancellation.OrderId,
				Reason = cancellation.Reason,
				OrderAmount = cancellation.OrderAmount,
				Status = cancellation.cancellationStatus.ToString(),
				RequestedAt = cancellation.RequestedAt,
				CancellationCharges = cancellation.CancellationCharges
			};
			return CancellationResponse;
		}
		private async Task NotifyCancellationAcceptedAsync(Cancellation cancellation)
		{
			if (cancellation.Order == null || cancellation.Order.customer == null)
			{
				return;
			}
			string Subject = $"Cancellation Request Update - Order #{cancellation.Order.OrderNumber}";

			var path = $"{webHostEnvironment.WebRootPath}/Templet/CancellationAcceptedEmail.html";
			var emailBody = await File.ReadAllTextAsync(path);

			Customer customer = cancellation.Order.customer;
			Order order = cancellation.Order;

			emailBody = emailBody.Replace("{Status}", "")
						 .Replace("{FirstName}", customer.applicationUser.FirstName)
						 .Replace("{LastName}", customer.applicationUser.LastName)
						 .Replace("{OrderNumber}", cancellation.Order.OrderNumber.ToString())
						 .Replace("{Reason}", cancellation.Reason ?? "N/A")
						 .Replace("{Remarks}", cancellation.Remarks ?? "N/A")
						 .Replace("{RequestedAt}", cancellation.RequestedAt.ToString("MMMM dd, yyyy HH:mm"))
						 .Replace("{ProcessedAt}", cancellation.ProcessedAt.HasValue
							 ? cancellation.ProcessedAt.Value.ToString("MMMM dd, yyyy HH:mm")
							 : "N/A")
						 .Replace("{OrderAmount}", cancellation.OrderAmount.ToString())
						 .Replace("{CancellationCharges}", cancellation.CancellationCharges?.ToString() ?? "0")
						 .Replace("{AmountToBeRefunded}", (cancellation.OrderAmount - (cancellation.CancellationCharges ?? 0)).ToString());

			// Send the email using the EmailService
			await emailSender.SendEmailAsync("", Subject, emailBody);
		}
		private async Task NotifyCancellationRejectionAsync(Cancellation cancellation)
		{
			if (cancellation.Order == null || cancellation.Order.customer == null)
			{
				return;
			}
			Customer customer = cancellation.Order.customer;
			Order order = cancellation.Order;
			string subject = $"Cancellation Request Rejected - Order #{cancellation.Order.OrderNumber}";

			var path = $"{webHostEnvironment.WebRootPath}/Templet/CancellationRejection.html";

			var emailbody = await File.ReadAllTextAsync(path);

				emailbody = emailbody.Replace("{FirstName}", customer.applicationUser.FirstName)
						 .Replace("{LastName}", customer.applicationUser.LastName)
						 .Replace("{OrderNumber}", cancellation.Order.OrderNumber.ToString())
						 .Replace("{Reason}", cancellation.Reason ?? "N/A")
						 .Replace("{Remarks}", cancellation.Remarks ?? "N/A")
						 .Replace("{RequestedAt}", cancellation.RequestedAt.ToString("MMMM dd, yyyy HH:mm"))
						 .Replace("{ProcessedAt}", cancellation.ProcessedAt.HasValue
							 ? cancellation.ProcessedAt.Value.ToString("MMMM dd, yyyy HH:mm")
							 : "N/A");

			await emailSender.SendEmailAsync("", subject, emailbody);

		}
	}
}