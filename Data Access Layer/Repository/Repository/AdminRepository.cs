
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Data_Access_Layer.Repository.Repository
{
	public class AdminRepository :GenericRepository<ApplicationUser>
	{
		private readonly UserManager<ApplicationUser> userManager ;
		private readonly EcommerceDbContext context;
		public AdminRepository(EcommerceDbContext context) :base(context) 
		{
			//	this.userManager = userManager;	
			// this.userManager = new UserManager<ApplicationUser>();
			this.context = context;
		}

		public async Task<List<ApplicationUser>> AllCustomer()
		{
		 var allUsers = await context.Users
			.Include(u => u.Customer) // Include Customer navigation property
			.ToListAsync();

			return allUsers;
		}
	}
}
