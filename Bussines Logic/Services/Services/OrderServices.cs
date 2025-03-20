
namespace Bussines_Logic.Services.Services
{
	public class OrderServices<D> : IGenericService<OrderResponseDTO, D> where D : class
	{
		public IUnitOfWork unitOfWork { get; }
		public OrderServices(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
		}

		public async Task<ApiResponse<OrderResponseDTO>> CreateAsync(D createDto)
		{
			if(createDto is OrderCreateDTO orderDto)
			{
				try {
					//Step 1: Check if the Customer Exists
					var customer=await unitOfWork.CustomerRepository.GetByIdAsync(orderDto.CustomerId);	
					if(customer is null)
						return new ApiResponse<OrderResponseDTO>(404,"Customer not valid");

					//Step 2: Verify the Billing Address
					var billingAddress = await unitOfWork.AddressRepository.GetByIdAsync(orderDto.BillingAddressId);
					if(billingAddress is null|| billingAddress.CustomerId != orderDto.CustomerId){
						return new ApiResponse<OrderResponseDTO>(400, "Billing Address is invalid or does not belong to the customer.");
					}

					//Step 3: Verify the Shipping Address
					var shippingAddress = await unitOfWork.AddressRepository.GetByIdAsync(orderDto.ShippingAddressId);
					if (shippingAddress is null || shippingAddress.CustomerId != orderDto.CustomerId)
					{
						return new ApiResponse<OrderResponseDTO>(400, "Shipping Address is invalid or does not belong to the customer.");
					}


					//Step 4: Start Preparing the Order
					var order = new Order() {
						OrderStatus=OrderStatus.Pending,	
						ShippingCost=10,
						BillingAddressId=orderDto.BillingAddressId,	
						ShippingAddressId=orderDto.ShippingAddressId,

					};
					return default;
				}
				catch (Exception ex) { return default; }	
			}
			else
			{
				return new ApiResponse<OrderResponseDTO>(400, "Invalid DTO type for Address registration.");
			}
		}

		public async Task<ApiResponse<OrderResponseDTO>> DeleteAsync(int id)
		{
			throw new NotImplementedException();
		}

		public async Task<ApiResponse<OrderResponseDTO>> GetByIdAsync(int id)
		{
			throw new NotImplementedException();
		}

		public async Task<ApiResponse<OrderResponseDTO>> UpdateAsync(D upateDto)
		{
			throw new NotImplementedException();
		}
	}
}
