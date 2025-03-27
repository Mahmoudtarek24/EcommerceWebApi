
using Data_Access_Layer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Bussines_Logic.Services.Services
{
	public class CustomerService<D> : IGenericService<CustomerResponseDTO, D> where D : class
	{
		public IUnitOfWork unitOfWork { get; }
		private readonly UserManager<ApplicationUser> userManager;
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly IImageService imageService;
		private readonly string CustomerImageFolderPath = @"\Customer";
		private readonly IEmailSender emailSender;
		private readonly EmailVerification emailVerification;
		private readonly EcommerceDbContext context;
		private readonly JWTSetting jWTSetting;
		public CustomerService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IImageService imageService
			, IEmailSender emailSender, EmailVerification emailVerification
			, SignInManager<ApplicationUser> signInManager, EcommerceDbContext context, IOptions<JWTSetting> options)
		{
			this.unitOfWork = unitOfWork;
			this.userManager = userManager;
			this.imageService = imageService;
			this.emailSender = emailSender;
			this.emailVerification = emailVerification;
			this.signInManager = signInManager;
			this.context = context;
			this.jWTSetting = options.Value;
		}
		//Register	Customer
		public async Task<ApiResponse<CustomerResponseDTO>> CreateAsync(D createDto)
		{
			if (createDto is CustomerRegistrationDTO customerDto)
			{
				UploadResult imageResult = null;
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
					if (customerDto.ImageData is not null)
					{
						imageResult = await imageService.UploadImage(customerDto.ImageData, CustomerImageFolderPath);
						if (!imageResult.IsUploaded)
							return new ApiResponse<CustomerResponseDTO>(400, imageResult.ErrorMessage);
						user.ProfileImage = imageResult.ImageName;

					}
					var result = await userManager.CreateAsync(user, customerDto.Password);
					if (!result.Succeeded)
					{
						var errorRespond = new ApiResponse<CustomerResponseDTO>();
						foreach (var error in result.Errors)
						{
							errorRespond.Errors.Add($"{error.Description} ,");
						}
						return errorRespond;
					}


					var customer = new Customer()
					{
						DateOfBirth = customerDto.DateOfBirth,
						UserId = user.Id,
					};
					await unitOfWork.CustomerRepository.Insert(customer);
					await unitOfWork.Save();
					await unitOfWork.Commit();

					var response = MapCustomerToDTO(customer);
					JwtSecurityToken jwtSecurityToken = await GenerateToken(user);

					response.Token=new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
					response.ExpiresOn = jwtSecurityToken.ValidTo;
					return new ApiResponse<CustomerResponseDTO>(200, response);
				}
				catch (Exception ex)
				{
					imageService.DeleteImage($"{CustomerImageFolderPath}/{imageResult?.ImageName}");
					await unitOfWork.RollBack();
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
				UploadResult imageResult = null;

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
					Customer.DateOfBirth = updateDTO.DateOfBirth;
					if (updateDTO.ImageData is not null)
					{
						imageService.DeleteImage($"{CustomerImageFolderPath}/{Customer.applicationUser.ProfileImage}");
						imageResult = await imageService.UploadImage(updateDTO.ImageData, CustomerImageFolderPath);
						if (!imageResult.IsUploaded)
							return new ApiResponse<CustomerResponseDTO>(400, imageResult.ErrorMessage);
						Customer.applicationUser.ProfileImage = imageResult.ImageName;

					}
					await userManager.UpdateAsync(Customer.applicationUser);
					await unitOfWork.Save();
					await unitOfWork.Commit();

					var response = MapCustomerToDTO(Customer);
					return new ApiResponse<CustomerResponseDTO>(200, response);
				}
				catch (Exception ex)
				{
					imageService.DeleteImage($"{CustomerImageFolderPath}/{imageResult?.ImageName}");
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
			try
			{
				string[] Includes = { "applicationUser" };
				var customer = await unitOfWork.CustomerRepository.GetEntityAsync(e => e.Id == id, Includes, true);
				if (customer is null)
					return new ApiResponse<CustomerResponseDTO>(404, "Customer not found.");

				var response = MapCustomerToDTO(customer);
				return new ApiResponse<CustomerResponseDTO>(200, response);

			}
			catch (Exception ex)
			{
				return new ApiResponse<CustomerResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}
		}

		public async Task<ApiResponse<CustomerResponseDTO>> DeleteAsync(int id)
		{
			string[] Includes = { "applicationUser" };
			var customer = await unitOfWork.CustomerRepository.GetEntityAsync(e => e.Id == id, Includes, true);
			if (customer is null)
				return new ApiResponse<CustomerResponseDTO>(404, "Customer not found.");

			try
			{
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
			catch (Exception ex)
			{
				await unitOfWork.RollBack();
				return new ApiResponse<CustomerResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}
		}
		public async Task<ApiResponse<ConfirmationResponseDTO>> ForgotPasswordAsync(ForgotPasswordDto forgotPassword)
		{
			try
			{
				var customer = await userManager.FindByEmailAsync(forgotPassword.Email);
				if (customer is null)
					return new ApiResponse<ConfirmationResponseDTO>(404, "Invalid Request");


				var code = emailVerification.GenerateCode(customer.Email);
				var emailBody = $@"
						Hi {customer.FirstName},
                          this Code Used It When You Reset Password   {code}
						";

				await emailSender.SendEmailAsync(customer.Email, "Password Reset Request", emailBody);
				return new ApiResponse<ConfirmationResponseDTO>(200, $"Check you email");
			}
			catch (Exception ex)
			{
				return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}
		}
		public async Task<ApiResponse<ConfirmationResponseDTO>> ResetPasswordAsync(ResetPasswordDTO resetPassword)
		{
			try
			{
				var user = await userManager.FindByEmailAsync(resetPassword.Email);
				if (user is null)
					return new ApiResponse<ConfirmationResponseDTO>(404, "Invalid Request");

				var checkcode = emailVerification.CheckCode(user.Email, resetPassword.code);
				if (!checkcode)
					return new ApiResponse<ConfirmationResponseDTO>(400, "Invalid Code");

				var oldpassword = user.PasswordHash;
				await userManager.RemovePasswordAsync(user);
				var result = await userManager.AddPasswordAsync(user, resetPassword.Password);
				if (result.Succeeded)
				{
					await userManager.UpdateAsync(user);
					var confirmationMessage = new ConfirmationResponseDTO
					{
						Message = "Password Reset successfully."
					};
					return new ApiResponse<ConfirmationResponseDTO>(200, confirmationMessage);
				}
				user.PasswordHash = oldpassword;
				await userManager.UpdateAsync(user);
				return new ApiResponse<ConfirmationResponseDTO>(400, $"Please Try On Another Time");
			}
			catch (Exception ex)
			{
				return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
			}
		}
		//public async Task<string> GetRecommendedProduct(int UserId)
		//{

		//}

		private CustomerResponseDTO MapCustomerToDTO(Customer customer)
		{
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

			return response;
		}


		public async Task<JwtSecurityToken> GenerateToken(ApplicationUser user)
		{
			var UserRoles = await userManager.GetRolesAsync(user);
			List<Claim> claims = new List<Claim>();
			if (UserRoles.Count > 0)
			{
				foreach (var role in UserRoles)
				{
					claims.Add(new Claim(ClaimTypes.Role, role));
				}
			}

			claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
			claims.Add(new Claim(ClaimTypes.Name, user.UserName));
			claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

			SymmetricSecurityKey symmetricSecurityKey =new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jWTSetting.Key));
			SigningCredentials signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

			JwtSecurityToken token = new JwtSecurityToken(
				issuer: jWTSetting.Issure,
				audience: jWTSetting.Audience,
				expires: DateTime.Now.AddDays(jWTSetting.DurationInDays),
				claims:claims,
                signingCredentials: signingCredentials
				);
			return token;
		}

	}
}



