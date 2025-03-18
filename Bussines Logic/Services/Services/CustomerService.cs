
namespace Bussines_Logic.Services.Services
{
	public class CustomerService<D> : IGenericService<CustomerResponseDTO, D> where D : class
	{
		public IUnitOfWork unitOfWork { get; }
		private readonly UserManager<ApplicationUser> userManager;
		public CustomerService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
		{
			this.unitOfWork = unitOfWork;
			this.userManager = userManager;
		}
		//Register	Customer
		public async Task<ApiResponse<CustomerResponseDTO>> CreateAsync(D createDto)
		{
			if (createDto is CustomerRegistrationDTO customerDto)
			{
				try
				{
					await unitOfWork.CreateTarsaction();
					var IsValidEmail = await userManager.FindByEmailAsync(customerDto.Email);
					if (IsValidEmail != null)
						return new ApiResponse<CustomerResponseDTO>(400, "Email is already in use.");

					var IsValidUserName = await userManager.FindByNameAsync(customerDto.UserName);
					if (IsValidUserName != null)
						return new ApiResponse<CustomerResponseDTO>(400, "UserName is already is use.");


					var user = new ApplicationUser()
					{
						Email = customerDto.Email,
						UserName = customerDto.UserName,
						FirstName = customerDto.FirstName,
						LastName = customerDto.LastName,
						PhoneNumber = customerDto.PhoneNumber,
					};

					var result = await userManager.CreateAsync(user, customerDto.Password);
					if (!result.Succeeded)
					{
						var response = new ApiResponse<CustomerResponseDTO>();
						foreach (var error in result.Errors)
						{
							response.Errors.Add($"{error.Description} ,");
						}
						return response;
					}


					var customer = new Customer()
					{
						//DateOfBirth = customerDto.DateOfBirth,
						UserId = user.Id,
					};
					await unitOfWork.CustomerRepository.Insert(customer);
					await unitOfWork.Save();
					await unitOfWork.Commit();

					var customerRespond = new CustomerResponseDTO()
					{
						CustomerId = customer.Id,
						DateOfBirth = customer.DateOfBirth,
						FirstName = user.FirstName,
						lastName = user.LastName,
						Email = user.Email,
						PhoneNumber = user.PhoneNumber,
						UserName= user.UserName,	
					};

					//TODO  Return Token
					return new ApiResponse<CustomerResponseDTO>(200, customerRespond);

				}
				catch (Exception ex)
				{
					unitOfWork.RollBack();
					return new ApiResponse<CustomerResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
				}
			}
			else
			{
				return new ApiResponse<CustomerResponseDTO>(400, "Invalid DTO type for customer registration.");
			}
		}

		public async Task<ApiResponse<LoginResponseDTO>> LoginAsync(LoginDTO loginDTO)
		{
			try
			{

				ApplicationUser user;
				//chech username ,or email is valaid
				var IsEmail = new EmailAddressAttribute().IsValid(loginDTO.Email);
				if (IsEmail)
				{
					user = await userManager.FindByEmailAsync(loginDTO.Email);
				}
				else
				{
					user = await userManager.FindByNameAsync(loginDTO.Email);
				}

				var result = await userManager.CheckPasswordAsync(user, loginDTO.Password);
				if (user is null || !result)
					return new ApiResponse<LoginResponseDTO>(401, "Invalid email or password.");

				//TODO  Return Token
				var Customer = await unitOfWork.CustomerRepository.GetEntityAsync(e => e.UserId == user.Id);
				var loginResponse = new LoginResponseDTO()
				{
					CustomerName = user.FirstName + " " + user.LastName,
					CustomerId = Customer.Id,
					Message = "Login successful."
				};
				return new ApiResponse<LoginResponseDTO>(200, loginResponse);
			}
			catch (Exception ex)
			{
				return new ApiResponse<LoginResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}
		}

		public async Task<ApiResponse<ConfirmationResponseDTO>> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO)
		{
			try
			{

				//need to change password

				string[] Includes = { "applicationUser" };
				var Customer = await unitOfWork.CustomerRepository.GetEntityAsync(e => e.Id == changePasswordDTO.CustomerId, Includes);


				if (Customer is null || Customer.applicationUser.Id is null)
					return new ApiResponse<ConfirmationResponseDTO>(404, "Customer not found or inactive.");

				var oldpassword = Customer.applicationUser.PasswordHash;
				await userManager.RemovePasswordAsync(Customer.applicationUser);
				var result = await userManager.AddPasswordAsync(Customer.applicationUser, changePasswordDTO.NewPassword);
				if (result.Succeeded)
				{
					await userManager.UpdateAsync(Customer.applicationUser);
					var confirmationMessage = new ConfirmationResponseDTO
					{
						Message = "Password changed successfully."
					};
					return new ApiResponse<ConfirmationResponseDTO>(200, confirmationMessage);
				}

				Customer.applicationUser.PasswordHash = oldpassword;
				await userManager.UpdateAsync(Customer.applicationUser);
				var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
				return new ApiResponse<ConfirmationResponseDTO>(400, $"Failed to change password: {errorMessage}");

			}
			catch (Exception ex) { return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}"); }
		}

		public async Task<ApiResponse<CustomerResponseDTO>> UpdateAsync(D upateDto)
		{
			if (upateDto is CustomerUpdateDTO updateDTO)
			{
				try
				{
					string[] Includes = { "applicationUser" };
					var Customer = await unitOfWork.CustomerRepository.GetEntityAsync(e => e.Id == updateDTO.CustomerId, Includes);
					if (Customer is null || Customer.applicationUser.Id is null)
						return new ApiResponse<CustomerResponseDTO>(404, "Customer not found or inactive.");

					//Check email ,UserName//
					var IsValidEmail = await userManager.FindByEmailAsync(updateDTO.Email);
					if (IsValidEmail != null)
						return new ApiResponse<CustomerResponseDTO>(400, "Email is already in use.");

					var IsValidUserName = await userManager.FindByNameAsync(updateDTO.UserName);
					if (IsValidUserName != null)
						return new ApiResponse<CustomerResponseDTO>(400, "UserName is already is use.");


					unitOfWork.CreateTarsaction();
					Customer.applicationUser.Email = updateDTO.Email;
					Customer.applicationUser.FirstName = updateDTO.FirstName;
					Customer.applicationUser.LastName = updateDTO.LastName;
					Customer.applicationUser.UserName = updateDTO.UserName;
					Customer.applicationUser.PhoneNumber = updateDTO.PhoneNumber;
					//Customer.DateOfBirth = updateDTO.DateOfBirth;

					await userManager.UpdateAsync(Customer.applicationUser);
					await unitOfWork.Save();
					await unitOfWork.Commit();

					var updateResponse = new CustomerResponseDTO
					{
						message = $"Customer with Id {updateDTO.CustomerId} , FullName {Customer.applicationUser.FirstName + " " + Customer.applicationUser.LastName} updated successfully."
					};
					return new ApiResponse<CustomerResponseDTO>(200, updateResponse);
				}
				catch (Exception ex)
				{
					unitOfWork.RollBack();
					return new ApiResponse<CustomerResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
				}
			}
			else
			{
					return new ApiResponse<CustomerResponseDTO>(500, $"Invalid DTO type for customer registration.");
			}
		}

		public async Task<ApiResponse<CustomerResponseDTO>> GetByIdAsync(int id)
		{
			try {
				string[] Includes = { "applicationUser" };
				var customer=await unitOfWork.CustomerRepository.GetEntityAsync(e=>e.Id==id,Includes,true);
				if (customer is null)
					return new  ApiResponse<CustomerResponseDTO>(404, "Customer not found.");

				var response = new CustomerResponseDTO()
				{
					DateOfBirth = customer.DateOfBirth,
					CustomerId = customer.Id,
					PhoneNumber = customer.applicationUser.PhoneNumber,
					Email = customer.applicationUser.Email,
					FirstName = customer.applicationUser.FirstName,
					UserName = customer.applicationUser.UserName,
					lastName = customer.applicationUser.LastName,
				};
				return new ApiResponse<CustomerResponseDTO>(200, response);

			}
			catch(Exception ex) {
				return new ApiResponse<CustomerResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}
		}

		public async Task<ApiResponse<CustomerResponseDTO>> DeleteAsync(int id)
		{
			string[] Includes = { "applicationUser" };
			var customer = await unitOfWork.CustomerRepository.GetEntityAsync(e => e.Id == id, Includes, true);
			if (customer is null)
				return new ApiResponse<CustomerResponseDTO>(404, "Customer not found.");

			try {
				await unitOfWork.CreateTarsaction();
				await unitOfWork.CustomerRepository.Delete(customer);
				await unitOfWork.Save();
				await unitOfWork.Commit();
				var updateResponse = new CustomerResponseDTO
				{
					message = $"Customer with Id {id} Deleted successfully."
				};
				return new ApiResponse<CustomerResponseDTO>(200, updateResponse);
			}
			catch(Exception ex) {
				await unitOfWork.RollBack();
				return new ApiResponse<CustomerResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}
		}
	}
}



