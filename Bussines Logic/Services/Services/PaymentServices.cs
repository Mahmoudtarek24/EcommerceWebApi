
namespace Bussines_Logic.Services.Services
{
	public class PaymentServices
	{
		private readonly IUnitOfWork unitOfWork;
		private readonly IEmailSender emailSender;
		private readonly IWebHostEnvironment webHostEnvironment;
		public PaymentServices(IUnitOfWork unitOfWork, IEmailSender emailSender, IWebHostEnvironment webHostEnvironment)
		{
			this.unitOfWork = unitOfWork;
			this.emailSender = emailSender;
			this.webHostEnvironment = webHostEnvironment;
		}
		public async Task<ApiResponse<PaymentResponseDTO>> ProcessPaymentAsync(PaymentRequestDTO paymentRequest)
		{
			try
			{
				await unitOfWork.CreateTarsaction();

				string[] Includes = { "payment" };
				var order = await unitOfWork.OrderRepository
						  .GetEntityAsync(e => e.OrderId == paymentRequest.OrderId && e.CustomerId == paymentRequest.CustomerId, Includes);

				if (order is null)
					return new ApiResponse<PaymentResponseDTO>(404, "Order not found.");

				Payment payment;
				if (order.payment != null)
				{
					// Allow retry only if previous payment failed and order status is still Pending
					if (order.payment.PaymentStatus == PaymentStatus.Failed && order.OrderStatus == OrderStatus.Pending)
					{
						payment = order.payment;
						payment.PaymentStatus = PaymentStatus.Completed;
						payment.PaymentDate = DateTime.Now;
						payment.Amount = order.TotalAmount;
						payment.TransactionId = null; // Clear previous transaction id
					}
					else
					{
						return new ApiResponse<PaymentResponseDTO>(400, "Order already has an associated payment.");
					}
				}
				else
				{
					// Create a new Payment record if none exists
					payment = new Payment
					{
						PaymentMethod = paymentRequest.PaymentMethod,
						OrderId = paymentRequest.OrderId,
						PaymentStatus = PaymentStatus.Pending,
						PaymentDate = DateTime.Now,
						Amount = order.TotalAmount,
					};
					await unitOfWork.PaymentRepository.Insert(payment);
				}

				// For non-COD payments, simulate payment processing
				if (!IsCashOnDelivery(paymentRequest.PaymentMethod))
				{
					var status = await SimulatePaymentGateway();
					payment.PaymentStatus = status;
					if (status == PaymentStatus.Completed)
					{
						order.OrderStatus = OrderStatus.Processing;
						payment.TransactionId = GenerateTransactionId();
					}
				}
				{
					payment.PaymentStatus = PaymentStatus.Pending;
					order.OrderStatus = OrderStatus.Processing;
					payment.TransactionId = GenerateTransactionId();
				}
				await unitOfWork.Save();
				await unitOfWork.Commit();
				if (order.OrderStatus == OrderStatus.Processing)
				{
					await SendOrderConfirmationEmailAsync(paymentRequest.OrderId);
				}

				var paymentResponse = MapPaymentToDTO(payment);
				
				return new ApiResponse<PaymentResponseDTO>(200, paymentResponse);

			}
			catch (Exception ex)
			{
				return new ApiResponse<PaymentResponseDTO>(500, "An unexpected error occurred while processing the payment.");
			}
		}

		
		public async Task<ApiResponse<PaymentResponseDTO>> GetPaymentByIdAsync(int paymentId)
		{
			try
			{
				var payment = await unitOfWork.PaymentRepository.GetByIdAsync(paymentId);
				if (payment is null)
					return new ApiResponse<PaymentResponseDTO>(404, "Payment not found");
				
				
				var paymentResponse = MapPaymentToDTO(payment);

				return new ApiResponse<PaymentResponseDTO>(200, paymentResponse);

			}
			catch (Exception ex)
			{
				return new ApiResponse<PaymentResponseDTO>(500, "An unexpected error occurred while processing the payment.");
			}
		}
		public async Task<ApiResponse<PaymentResponseDTO>> GetPaymentByOrderIdAsync(int orderId)
		{
			try
			{
				string[] Includes = { "payment" };

				var order = await unitOfWork.OrderRepository.GetEntityAsync(e => e.OrderId == orderId, Includes);
				if (order is null )
					return new ApiResponse<PaymentResponseDTO>(404, "Order Not found");
				if(order.payment is null)
					return new ApiResponse<PaymentResponseDTO>(404, "No payment occure for that order");

				var paymentResponse = MapPaymentToDTO(order.payment);

				return new ApiResponse<PaymentResponseDTO>(200, paymentResponse);
			}
			catch (Exception ex) 
			{
				return new ApiResponse<PaymentResponseDTO>(500, "An unexpected error occurred while processing the payment.");
			}
		}
		public async Task<ApiResponse<ConfirmationResponseDTO>> CompleteCODPaymentAsync(CODPaymentUpdateDTO codPaymentUpdateDTO)
		{
			try
			{
				await unitOfWork.CreateTarsaction();
				string[] Includes = { "Order" };
				var payment = await unitOfWork.PaymentRepository.GetEntityAsync(e => e.PaymentId == codPaymentUpdateDTO.PaymentId, Includes);
				if (payment is null)
					return new ApiResponse<ConfirmationResponseDTO>(404, "Payment not found.");
				if (payment.Order is null)
					return new ApiResponse<ConfirmationResponseDTO>(404, "Order not found.");

				if(payment.Order.OrderStatus!=OrderStatus.Shipped)
					return new ApiResponse<ConfirmationResponseDTO>(400, $"Order Cant not marked as as Delivered from {payment.Order.OrderStatus} State");

				payment.PaymentStatus = PaymentStatus.Completed;
				payment.Order.OrderStatus = OrderStatus.Delivered;

				await unitOfWork.Save();
				await unitOfWork.Commit();

				var confirmation = new ConfirmationResponseDTO
				{
					Message = $"COD Payment for Order ID {payment.Order.OrderId} has been marked as 'Completed' and the order status updated to 'Delivered'."
				};
				return new ApiResponse<ConfirmationResponseDTO>(200, confirmation);
			}
			catch (Exception ex)
			{
				await unitOfWork.RollBack();
				return new ApiResponse<ConfirmationResponseDTO>(500, "An unexpected error occurred while processing the payment.");
			}

		}
		private PaymentResponseDTO MapPaymentToDTO(Payment payment)
		{
			var paymentResponse = new PaymentResponseDTO()
			{
				Amount = payment.Amount,
				OrderId = payment.OrderId,
				PaymentDate = payment.PaymentDate,
				Status = payment.PaymentStatus,
				TransactionId = payment.TransactionId,
				PaymentMethod = payment.PaymentMethod,
				PaymentId = payment.PaymentId,
				Message = "Payment Data"
			};
			return paymentResponse;
		}

		private string GenerateTransactionId()
		{
			return $"TXN-{Guid.NewGuid().ToString("N").ToUpper().Substring(0, 12)}";
		}
		private bool IsCashOnDelivery(string paymentMethod)
		{
			return paymentMethod.Equals("CashOnDelivery", StringComparison.OrdinalIgnoreCase) ||
				   paymentMethod.Equals("COD", StringComparison.OrdinalIgnoreCase);
		}
		private async Task<PaymentStatus> SimulatePaymentGateway()
		{
			int number = Random.Shared.Next(1, 100);

			switch (number)
			{
				case int n when n <= 85:
					return PaymentStatus.Completed;
				default:
					return PaymentStatus.Failed;

			}
		}
		public async Task SendOrderConfirmationEmailAsync(int orderId)
		{
			var order=await unitOfWork.OrderRepository.OrdersEmailDate(orderId);
			Customer customer = order.customer;
			Address address = order.BillingAddress;
			Address addressSh = order.ShippingAddress;
			Payment payment = order.payment;

			var filePath = $"{webHostEnvironment.WebRootPath}/Templet/Email.html";
		
			var emailBody=await File.ReadAllTextAsync(filePath);

			string orderItemsHtml = string.Join("", order.orderItems.Select(item => $@"
                <tr>
                  <td style='padding: 8px; border: 1px solid #dddddd;'>{item.product.ProductName}</td>
                  <td style='padding: 8px; border: 1px solid #dddddd;'>{item.Quantity}</td>
                  <td style='padding: 8px; border: 1px solid #dddddd;'>{item.UnitPrice:C}</td>
                  <td style='padding: 8px; border: 1px solid #dddddd;'>{item.Discount:C}</td>
                  <td style='padding: 8px; border: 1px solid #dddddd;'>{item.TotalPrice:C}</td>
                </tr>
            ").ToList());

			emailBody =emailBody.Replace("{CustomerFirstName}", customer.applicationUser.FirstName)
				.Replace("{CustomerLastName}", customer.applicationUser.LastName)
				.Replace("{OrderNumber}", order.OrderNumber)
				.Replace("{OrderDate}", order.OrderDate.ToString("MMMM dd, yyyy"))
				.Replace("{SubTotal}", order.TotalBaseAmount.ToString("C"))
				.Replace("{Discount}", order.TotalDiscountAmount.ToString("C"))
				.Replace("{ShippingCost}", order.ShippingCost.ToString("C"))
				.Replace("{TotalAmount}", order.TotalAmount.ToString("C"))
				.Replace("{OrderItems}", orderItemsHtml)
				.Replace("{BillingAddressLine1}", address.AddressLine1)
				.Replace("{BillingAddressLine2}", string.IsNullOrWhiteSpace(address.AddressLine2) ? "" : address.AddressLine2 + "<br/>")
				.Replace("{BillingCity}", address.Area)
				.Replace("{BillingState}", address.State)
				.Replace("{BillingPostalCode}", address.PostalCode)
				.Replace("{BillingCountry}", address.Governorate)
				.Replace("{ShippingAddressLine1}", addressSh.AddressLine1)
				.Replace("{ShippingAddressLine2}", string.IsNullOrWhiteSpace(addressSh.AddressLine2) ? "" : addressSh.AddressLine2 + "<br/>")
				.Replace("{ShippingCity}", addressSh.Area)
				.Replace("{ShippingState}", addressSh.State)
				.Replace("{ShippingPostalCode}", addressSh.PostalCode)
				.Replace("{ShippingCountry}", addressSh.Governorate)
				.Replace("{PaymentMethod}", payment?.PaymentMethod ?? "N/A")
				.Replace("{PaymentDate}", payment != null ? payment.PaymentDate.ToString("MMMM dd, yyyy HH:mm") : "N/A")
				.Replace("{TransactionId}", payment?.TransactionId ?? "N/A")
				.Replace("{PaymentStatus}", payment?.PaymentStatus.ToString() ?? "N/A"); ;
			    string subject = $"Order Confirmation - {order.OrderNumber}";
			    await emailSender.SendEmailAsync("", subject, emailBody);
		}
	}
}
