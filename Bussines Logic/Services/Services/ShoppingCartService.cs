
namespace Bussines_Logic.Services.Services
{
	public class ShoppingCartService
	{
		private readonly IUnitOfWork unitOfWork;
		public ShoppingCartService(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
		}

		// Adds a product to the customer's cart. (customer have cart ,cart have many product)
		// Creates an active cart if one does not exist.
		// If the product already exists in the cart, its quantity is updated.
		public async Task<ApiResponse<CartResponseDTO>> AddToCartAsync(AddToCartDTO AddDto)
		{
			//check if i have customerId
			var customer = await unitOfWork.CustomerRepository.GetByIdAsync(AddDto.CustomerId);
			if (customer == null)
				return new ApiResponse<CartResponseDTO>(404, "Customer not found");

			var product = await unitOfWork.ProductRepository.GetByIdAsync(AddDto.ProductId);
			if (product is null || product.IsDeleted || !product.IsAvailable)
			{
				var result = product is null ? "Product not found" : product.IsDeleted ? "Product Was Deleted" : "product Not Available";
				return new ApiResponse<CartResponseDTO>(404, result);
			}
			//Quentity is Bigger than on stock
			if (product.StockQuantity < AddDto.Quantity)
				return new ApiResponse<CartResponseDTO>(400, $"Only {product.StockQuantity} units of {product.ProductName} are available.");

			//var CheckResult = await AvailableData(AddDto.CustomerId, AddDto.ProductId, AddDto.Quantity);
			//if (CheckResult != null)
			//	return new ApiResponse<CartResponseDTO>(404, CheckResult);

			try
			{
				//this customer have cart before 
				var cart = await unitOfWork.CartRepository.RetriveActiveCart(AddDto.CustomerId);

				await unitOfWork.CreateTarsaction();
				// If no active cart exists, create a new cart.
				if (cart is null)
				{
					cart = new Cart()
					{
						IsCheckedOut = false,
						CreatedAt = DateTime.Now,
						customerId = AddDto.CustomerId,
					};
					await unitOfWork.CartRepository.Insert(cart);
					await unitOfWork.Save();
				}

				// Check if the product is already in the cart.
				var existingCartItem = cart.CartItems.FirstOrDefault(e => e.ProductId == AddDto.ProductId);

				// Calculate discount per unit, if applicable.
				var discountPerUnit = product.DiscountPercentage > 0 ? (product.Price * product.DiscountPercentage / 100) : 0;
				var totalDiscount = discountPerUnit * AddDto.Quantity;

				if (existingCartItem != null)
				{
					//Add number of  Propuct to product added before
					//Check Quantity of customer available on stoc
					if (existingCartItem.Quantity + AddDto.Quantity > product.StockQuantity)
						return new ApiResponse<CartResponseDTO>(400, $"Adding {AddDto.Quantity} exceeds available stock.");

					//add new Quantity of product to selected product
					existingCartItem.Quantity += AddDto.Quantity;
					existingCartItem.Discount += totalDiscount;
					existingCartItem.UnitPrice = product.Price;	
					existingCartItem.TotalPrice = (existingCartItem.UnitPrice * existingCartItem.Quantity) - existingCartItem.Discount;
					existingCartItem.UpdatedAt = DateTime.Now;

					//Reducing the quantity of the product from the Stock after adding to cart
					product.StockQuantity -= AddDto.Quantity;
					product.IsAvailable = product.StockQuantity == 0 ? false : true;

					await unitOfWork.Save();


				}//if product not on the cart (add new product to cart item)
				else
				{
					var cartItem = new CartItem()
					{
						CartId = cart.cartId,
						Discount = totalDiscount,
						ProductId = AddDto.ProductId,
						CreatedAt = DateTime.Now,
						Quantity = AddDto.Quantity,
						UnitPrice = product.Price,
					    TotalPrice=(product.Price*AddDto.Quantity)- totalDiscount
					};
					await unitOfWork.CartItemRepository.Insert(cartItem);

					product.StockQuantity -= AddDto.Quantity;
					product.IsAvailable = product.StockQuantity == 0 ? false : true;
					await unitOfWork.Save();
				}

				cart.UpdatedAt = DateTime.Now;
				await unitOfWork.Save();
				await unitOfWork.Commit();

				// Reload the cart with the latest details (including related CartItems and Products)
				// cart = await unitOfWork.CartRepository.RetriveLastUpdateOrAddedCartItemAndProduct(cart.cartId);(can used same object)
				var crt = await unitOfWork.CartRepository.RetriveLastUpdateOrAddedCartItemAndProduct(cart.cartId);

				// Map the cart entity to the DTO, which includes price calculations.
				var respond = await MapCartToDTO(crt);

				return new ApiResponse<CartResponseDTO>(200, respond);
			}
			catch (Exception ex)
			{
				await unitOfWork.RollBack();
				return new ApiResponse<CartResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}
		}

		// Updates the quantity of a specific item in the customer's cart.
		public async Task<ApiResponse<CartResponseDTO>> UpdateCartItemAsync(UpdateCartItemDTO updateCartItemDTO)
		{
			var CheckResult = await AvailableData(updateCartItemDTO.CustomerId, updateCartItemDTO.ProductId, updateCartItemDTO.Quantity);
			if (CheckResult != null)
				return new ApiResponse<CartResponseDTO>(404, CheckResult);

			var CartOfCustomer = await unitOfWork.CartRepository.RetriveActiveCart(updateCartItemDTO.CustomerId);
			if (CartOfCustomer is null)
			{
				return new ApiResponse<CartResponseDTO>(404, "Active cart not found.");
			}

			try
			{
				var product = await unitOfWork.ProductRepository.GetByIdAsync(updateCartItemDTO.ProductId);

				await unitOfWork.CreateTarsaction();

				var existingCartItem = CartOfCustomer.CartItems.FirstOrDefault(e => e.ProductId == updateCartItemDTO.ProductId);
				var discountPerUnit = product.DiscountPercentage > 0 ? (product.Price * product.DiscountPercentage / 100) : 0;
				var totalDiscount = discountPerUnit * updateCartItemDTO.Quantity;
				
				if (existingCartItem is null)
				{
					CartItem cartItem = new CartItem()
					{
						CartId = CartOfCustomer.cartId,
						ProductId = updateCartItemDTO.ProductId,
						Discount = discountPerUnit,
						UnitPrice = product.Price,
						TotalPrice = (product.Price * updateCartItemDTO.Quantity) - totalDiscount,
						Quantity = updateCartItemDTO.Quantity,
						CreatedAt = DateTime.Now,
					};
					await unitOfWork.CartItemRepository.Insert(cartItem);

					product.StockQuantity -= updateCartItemDTO.Quantity;
					product.IsAvailable = product.StockQuantity == 0 ? false : true;

					await unitOfWork.Save();
				}
				if (existingCartItem.Quantity + updateCartItemDTO.Quantity > product.StockQuantity)
					return new ApiResponse<CartResponseDTO>(400, $"Adding {updateCartItemDTO.Quantity} exceeds available stock.");

				existingCartItem.Quantity += updateCartItemDTO.Quantity;
				existingCartItem.Discount += discountPerUnit;
				existingCartItem.TotalPrice = (existingCartItem.UnitPrice - existingCartItem.Discount) * existingCartItem.Quantity;
				existingCartItem.UpdatedAt = DateTime.Now;

				product.StockQuantity -= updateCartItemDTO.Quantity;
				product.IsAvailable = product.StockQuantity == 0 ? false : true;

				CartOfCustomer.UpdatedAt = DateTime.Now;
				await unitOfWork.Save();
				await unitOfWork.Commit();

				var respond = await MapCartToDTO(CartOfCustomer);

				return new ApiResponse<CartResponseDTO>(200, respond);

			}
			catch (Exception ex)
			{
				await unitOfWork.RollBack();
				return new ApiResponse<CartResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}
		}
		public async Task<ApiResponse<CartResponseDTO>> RemoveCartItemAsync(RemoveCartItemDTO removeCartItemDTO)
		{
			var cart = await unitOfWork.CartRepository.RetriveActiveCart(removeCartItemDTO.CustomerId);
			if (cart is null)
				return new ApiResponse<CartResponseDTO>(404, $"Didnt Found Cart for Customer {removeCartItemDTO.CustomerId}");

			var cartItem = cart.CartItems.FirstOrDefault(e => e.CartItemId == removeCartItemDTO.CartItemId);
			if (cartItem is null)
				return new ApiResponse<CartResponseDTO>(404, $"Didnt Found Cart Item for Customer {removeCartItemDTO.CustomerId} and Cart Item {removeCartItemDTO.CartItemId}");

			try
			{
				await unitOfWork.CreateTarsaction();

				cartItem.Product.StockQuantity += cartItem.Quantity;
				cartItem.Product.IsAvailable = cartItem.Product.StockQuantity == 0 ? false : true;

				await unitOfWork.CartItemRepository.Delete(cartItem);
				cart.UpdatedAt = DateTime.Now;

				await unitOfWork.Save();
				await unitOfWork.Commit();

				var respond = await MapCartToDTO(cart);

				return new ApiResponse<CartResponseDTO>(200, respond);
			}
			catch (Exception ex)
			{
				await unitOfWork.RollBack();
				return new ApiResponse<CartResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}
		}
		public async Task<ApiResponse<CartResponseDTO>> GetCartByCustomerIdAsync(int customerId)
		{
			try
			{
				var cart = await unitOfWork.CartRepository.RetriveActiveCart(customerId);
				if (cart is null)
				{
					var emptyCartDTO = new CartResponseDTO();
					return new ApiResponse<CartResponseDTO>(200, emptyCartDTO);
				}
				var respond = await MapCartToDTO(cart);

				return new ApiResponse<CartResponseDTO>(200, respond);

			}
			catch (Exception ex)
			{
				return new ApiResponse<CartResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}
		}
		public async Task<ApiResponse<ConfirmationResponseDTO>> ClearCartAsync(int customerId)
		{
			try
			{
				await unitOfWork.CreateTarsaction();
				var cart = await unitOfWork.CartRepository.RetriveActiveCart(customerId);
				if (cart is null)
					return new ApiResponse<ConfirmationResponseDTO>(404, "Active cart not found.");

				foreach (var cartItem in cart.CartItems.ToList())
				{
					cartItem.Product.StockQuantity += cartItem.Quantity;
					cartItem.Product.IsAvailable = cartItem.Product.StockQuantity == 0 ? false : true;

					await unitOfWork.CartItemRepository.Delete(cartItem);
				}

				await unitOfWork.Save();

				await unitOfWork.CartRepository.Delete(cart);
				await unitOfWork.Save();
				await unitOfWork.Commit();
				var confirmation = new ConfirmationResponseDTO
				{
					Message = "Cart has been cleared successfully."
				};
				return new ApiResponse<ConfirmationResponseDTO>(200, confirmation);
			}
			catch (Exception ex) { return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}"); }	
		}

		private async Task<string> AvailableData(int customerId, int ProductId, int Quantity)
		{
			string check = null;

			var customer = await unitOfWork.CustomerRepository.GetByIdAsync(customerId);
			if (customer == null)
				return "Customer not found";

			var product = await unitOfWork.ProductRepository.GetByIdAsync(ProductId);
			if (product is null || product.IsDeleted || !product.IsAvailable)
			{
				var result = product is null ? "Product not found" : product.IsDeleted ? "Product Was Deleted" : "product Not Available";
				return result;
			}
			//Quentity is Bigger than on stock
			if (product.StockQuantity < Quantity)
				return $"Only {product.StockQuantity} units of {product.ProductName} are available.";

			return check;
		}
		private async Task<CartResponseDTO> MapCartToDTO(Cart cart)
		{

			List<CartItemResponseDTO> cartItemResponseDTO = new List<CartItemResponseDTO>();
			decimal totalBasePrice = 0;
			decimal totalDiscount = 0;
			decimal totalAmount = 0;

			if (cart.CartItems.Count > 0)
			{

				foreach (var itemCart in cart.CartItems)
				{

					CartItemResponseDTO cartItem = new CartItemResponseDTO()
					{
						ProductId = itemCart.ProductId,
						ProductName = itemCart.Product.ProductName,
						Quantity = itemCart.Quantity,                        /////quentity for each product indvidule
						TotalPrice = itemCart.TotalPrice,                   /////TotalPrice for each product indvidule
						Discount = itemCart.Discount,
						UnitPrice = itemCart.UnitPrice,
						CartItemId = itemCart.CartItemId,
					};
					totalBasePrice += cartItem.Quantity * cartItem.UnitPrice;
					totalDiscount += cartItem.Discount;      //tottal sum discount for all product
					totalAmount += cartItem.TotalPrice;                           // Sum of final prices after discount


					cartItemResponseDTO.Add(cartItem);
				}
			}
			CartResponseDTO cartResponseDTO = new CartResponseDTO()
			{
				CartId = cart.cartId,
				CustomerId = cart.customerId,
				IsCheckedOut = cart.IsCheckedOut,
				CreatedAt = cart.CreatedAt,
				UpdatedAt = cart.UpdatedAt,
				TotalBasePrice = totalBasePrice,
				TotalDiscount = totalDiscount,
				TotalAmount = totalAmount,
				CartItems = cartItemResponseDTO
			};

			return cartResponseDTO;
		}
	}
}
