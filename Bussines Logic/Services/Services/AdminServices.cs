
namespace Bussines_Logic.Services.Services
{
	public class AdminServices
	{
		private readonly UserManager<ApplicationUser> userManager;

		private readonly IUnitOfWork unitOfWork;

		public AdminServices(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
		{
			this.unitOfWork = unitOfWork;
			this.userManager = userManager;
		//	CreateAdmin();
		}

		private async Task CreateAdmin()
		{
			ApplicationUser Admin = new ApplicationUser()
			{
				FirstName = "admin",
				LastName = "admin",
				Email = "test1@gmail.com",
				UserName = "admin1",
			};
			var IsAdminExist = await userManager.FindByNameAsync(Admin.UserName);
			if (IsAdminExist is null)
			{
				var result = await userManager.CreateAsync(Admin, "12345678Mm#");
				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(Admin, "Admin");
				}
			}
		}

		public async Task<ApiResponse<List<AllCustomerRespond>>> GetAllCustomer()
		{
			try
			{
				var customers = await unitOfWork.AdminRepository.AllCustomer();
				if (customers.Count() == 0)
					return new ApiResponse<List<AllCustomerRespond>>(200, "Still no Customer");

				List<AllCustomerRespond> customerResponds = new List<AllCustomerRespond>();

				foreach (var customer in customers)
				{
					AllCustomerRespond Customer = new AllCustomerRespond()
					{
						Email = customer.Email,
						FirstName = customer.FirstName,
						lastName = customer.LastName,
						PhoneNumber = customer.FirstName,
						UserName = customer.UserName,
						Id = customer.Customer.Id,
						DateOfBirth = customer.Customer.DateOfBirth,
					};
					customerResponds.Add(Customer);	
				}
				return new ApiResponse<List<AllCustomerRespond>>(200, customerResponds);
			}
			catch (Exception ex) { 
					return new ApiResponse<List<AllCustomerRespond>>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");

			}

		}
	}


}
