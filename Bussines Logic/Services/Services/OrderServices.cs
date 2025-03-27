
using Data_Access_Layer.Models;
using System.Linq;

namespace Bussines_Logic.Services.Services
{
	public class OrderServices<D>
	{
		public static readonly Dictionary<OrderStatus, List<OrderStatus>> AllowedStatusTransitions =
			new Dictionary<OrderStatus, List<OrderStatus>>()
			{
			{ OrderStatus.Pending, new List<OrderStatus> { OrderStatus.Processing, OrderStatus.Canceled } },
			{ OrderStatus.Processing, new List<OrderStatus> { OrderStatus.Shipped, OrderStatus.Canceled } },
			{ OrderStatus.Shipped, new List<OrderStatus> { OrderStatus.Delivered } },
			{ OrderStatus.Delivered, new List<OrderStatus>() }, // Terminal state
            { OrderStatus.Canceled, new List<OrderStatus>() }   // Terminal state
            };

		public IUnitOfWork unitOfWork { get; }
		public OrderServices(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
		}
		public async Task<ApiResponse<ConfirmationResponseDTO>> UpdateOrderStatusAsync(OrderStatusUpdateDTO upateOrderStausDto)
		{
			try
			{
				var order = await unitOfWork.OrderRepository.GetByIdAsync(upateOrderStausDto.OrderId);
				if (order is null)
					return new ApiResponse<ConfirmationResponseDTO>(404, "Order Not Found");

				var currentStatus = order.OrderStatus;
				//conver incomming data to enum
				if (!Enum.TryParse<OrderStatus>(upateOrderStausDto.OrderStatus, true, out var newStatus))
					return new ApiResponse<ConfirmationResponseDTO>(404, "Invalid order status provided.");

				//Old status is exist and get her valid value
				if (!AllowedStatusTransitions.TryGetValue(currentStatus, out var allowedStatuses))
					return new ApiResponse<ConfirmationResponseDTO>(500, "Current order status is invalid.");

				if (!allowedStatuses.Contains(newStatus))
					return new ApiResponse<ConfirmationResponseDTO>(400, $"Cannot change order status from {currentStatus} to {newStatus}.");

				await unitOfWork.CreateTarsaction();
				order.OrderStatus = newStatus;
				order.LastUpdateOn = DateTime.Now;

				await unitOfWork.Save();
				await unitOfWork.Commit();

				var confirmation = new ConfirmationResponseDTO
				{
					Message = $"Order Status with Id {upateOrderStausDto.OrderId} and Status : {order.OrderStatus} updated successfully."
				};
				return new ApiResponse<ConfirmationResponseDTO>(200,confirmation);	

			}
			catch (Exception ex)
			{
				return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}
		}
		public async Task<ApiResponse<OrderResponseDTO>> CreateAsync(D createDto)
		{
			if (createDto is OrderCreateDTO orderDto)
			{
				try
				{
					//Step 1: Check if the Customer Exists
					var customer = await unitOfWork.CustomerRepository.GetByIdAsync(orderDto.CustomerId);
					if (customer is null)
						return new ApiResponse<OrderResponseDTO>(404, "Customer not valid");

					//Step 2: Verify the Billing Address
					var billingAddress = await unitOfWork.AddressRepository.GetByIdAsync(orderDto.BillingAddressId);
					if (billingAddress is null || billingAddress.CustomerId != orderDto.CustomerId)
					{
						return new ApiResponse<OrderResponseDTO>(400, "Billing Address is invalid or does not belong to the customer.");
					}

					await unitOfWork.CreateTarsaction();

					//Step 3: Verify the Shipping Address
					var shippingAddress = await unitOfWork.AddressRepository.GetByIdAsync(orderDto.ShippingAddressId);
					if (shippingAddress is null || shippingAddress.CustomerId != orderDto.CustomerId)
					{
						return new ApiResponse<OrderResponseDTO>(400, "Shipping Address is invalid or does not belong to the customer.");
					}


					// List to hold order items.
					var orderItems = new List<OrderItem>();

					decimal totalBaseAmount = 0;
					decimal totalDiscountAmount = 0;
					decimal shippingCost = 10.00m;
					decimal totalAmount = 0;

					//Step 4: Start Preparing the Order
					foreach (var itemDto in orderDto.OrderItems)
					{
						var product = await unitOfWork.ProductRepository.GetByIdAsync(itemDto.ProductId);

						if (product is null || product.IsDeleted || !product.IsAvailable)
						{
							var result = product is null ? "Product not found" : product.IsDeleted ? "Product Was Deleted" : "product Not Available";
							return new ApiResponse<OrderResponseDTO>(404, result);
						}
						// Check if sufficient stock is available.
						if (product.StockQuantity < itemDto.Quantity)
						{
							return new ApiResponse<OrderResponseDTO>(400, $"Insufficient stock for product {product.ProductName}.");
						}

						//	var discount = product.DiscountPercentage > 0 ? product.Price * product.DiscountPercentage / 100 : 0;

						var discountPerUnit = product.DiscountPercentage > 0 ? (product.Price * product.DiscountPercentage / 100) : 0;
						var totalDiscount = discountPerUnit * itemDto.Quantity;

						decimal basePrice = itemDto.Quantity * product.Price;

						var orderItem = new OrderItem()
						{
							Quantity = itemDto.Quantity,
							UnitPrice = product.Price,
							Discount = totalDiscount,
							ProductId = product.ProductId,
							TotalPrice = (product.Price * itemDto.Quantity) - totalDiscount,
						};
						orderItems.Add(orderItem); //OrderItemId  OrderId

						totalDiscountAmount += totalDiscount;
						totalBaseAmount += basePrice;

						product.StockQuantity -= itemDto.Quantity;
						product.IsAvailable = product.StockQuantity == 0 ? false : true;
					}

					// Calculate the final total amount.
					totalAmount = totalBaseAmount - totalDiscountAmount + shippingCost;



					var order = new Order()
					{
						OrderStatus = OrderStatus.Pending,
						ShippingCost = shippingCost,
						BillingAddressId = orderDto.BillingAddressId,
						ShippingAddressId = orderDto.ShippingAddressId,
						orderItems = orderItems,
						CustomerId = orderDto.CustomerId,
						TotalAmount = totalAmount,
						TotalDiscountAmount = totalDiscountAmount,
						TotalBaseAmount = totalBaseAmount,
						OrderDate = DateTime.Now,
						CreateOn = DateTime.Now,
						OrderNumber = GenerateOrderNumber()
					};
					await unitOfWork.OrderRepository.Insert(order);

					var cart = await unitOfWork.CartRepository.RetriveActiveCart(orderDto.CustomerId);
					if (cart != null)
					{
						cart.IsCheckedOut = true;
						cart.UpdatedAt = DateTime.Now;
					}

					await unitOfWork.Save();
					await unitOfWork.Commit();

					var respond = MapOrderToDTO(order);
					respond.Message = "Order Data";

					return new ApiResponse<OrderResponseDTO>(200, respond);
				}
				catch (Exception ex)
				{
					await unitOfWork.RollBack();
					return new ApiResponse<OrderResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
				}
			}
			else
			{
				return new ApiResponse<OrderResponseDTO>(400, "Invalid DTO type for Address registration.");
			}
		}

		public async Task<ApiResponse<List<OrderResponseDTO>>> GetByIdAsync(int id)
		{
			try
			{
				string[] Includes = { "orderItems" };
				var orders = await unitOfWork.OrderRepository.GetAllEntitiesAsync(e => e.OrderId == id, Includes, true);
				if (orders.Count() == 0)
					return new ApiResponse<List<OrderResponseDTO>>(404, "Order Not Found");


				List<OrderResponseDTO> orderListRespond = new List<OrderResponseDTO>();

				foreach (var order in orders.ToList())
				{
					var respond = MapOrderToDTO(order);
					respond.Message = "Order Data";
					orderListRespond.Add(respond);
				}

				return new ApiResponse<List<OrderResponseDTO>>(200, orderListRespond);

			}
			catch (Exception ex)
			{
				return new ApiResponse<List<OrderResponseDTO>>(200, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}
		}
		public async Task<ApiResponse<List<OrderResponseDTO>>> GetAllOrdersAsync()
		{
			try
			{
				var orders = await unitOfWork.OrderRepository.OrdersData();

				List<OrderResponseDTO> orderListRespond = new List<OrderResponseDTO>();

				foreach (var order in orders.ToList())
				{
					var respond = MapOrderToDTO(order);
					respond.Message = "Order Data";
					orderListRespond.Add(respond);
				}

				return new ApiResponse<List<OrderResponseDTO>>(200, orderListRespond);

			}
			catch (Exception ex)
			{
				return new ApiResponse<List<OrderResponseDTO>>(200, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}
		}
		public async Task<ApiResponse<List<OrderResponseDTO>>> GetOrdersByCustomerAsync(int CustomerId)
		{
			try
			{
				var orders = await unitOfWork.OrderRepository.OrdersForCustome(CustomerId);
				if (orders.Count() == 0)
					return new ApiResponse<List<OrderResponseDTO>>(400, "Customer dint execute Order");

				List<OrderResponseDTO> orderListRespond = new List<OrderResponseDTO>();

				foreach (var order in orders.ToList())
				{
					var respond = MapOrderToDTO(order);
					respond.Message = "Order Data";
					orderListRespond.Add(respond);
				}

				return new ApiResponse<List<OrderResponseDTO>>(200, orderListRespond);

			}
			catch (Exception ex)
			{
				return new ApiResponse<List<OrderResponseDTO>>(200, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}

		}

		private string GenerateOrderNumber()
		{
			var random = new Random();
			return $"ORD-{DateTime.UtcNow.ToString("yyyyMMdd-HHmmss")}-{random.Next(1000, 9999)}";
		}
		private OrderResponseDTO MapOrderToDTO(Order order)
		{
			List<OrderItemRespondDto> orderItemResponds = new List<OrderItemRespondDto>();

			foreach (var orderItem in order.orderItems.ToList())
			{
				OrderItemRespondDto orderItem1 = new OrderItemRespondDto()
				{
					OrderItemId = orderItem.OrderId,
					Quantity = orderItem.Quantity,
					UnitPrice = orderItem.UnitPrice,
					Discount = orderItem.Discount,
					TotalPrice = orderItem.TotalPrice,
					ProductId = orderItem.ProductId,
				};
				orderItemResponds.Add(orderItem1);
			}

			OrderResponseDTO orderResponse = new OrderResponseDTO()
			{
				OrderId = order.OrderId,
				OrderNumber = order.OrderNumber,
				OrderDate = order.OrderDate,
				OrderStatus = order.OrderStatus,
				BillingAddressId = order.BillingAddressId,
				ShippingAddressId = order.ShippingAddressId,
				CustomerId = order.CustomerId,
				IsDeleted = order.IsDeleted,
				CreateOn = order.CreateOn,
				LastUpdateOn = order.LastUpdateOn,
				orderItem = orderItemResponds,
				TotalAmount = order.TotalAmount,
				TotalBaseAmount = order.TotalBaseAmount,
				ShippingCost = order.ShippingCost,
				TotalDiscountAmount = order.TotalDiscountAmount,
			};

			return orderResponse;
		}
	}
}
