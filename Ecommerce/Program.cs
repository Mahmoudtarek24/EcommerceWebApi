using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddDbContext<EcommerceDbContext>(option =>
			{
				option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
			});
			builder.Services.AddIdentity<ApplicationUser,IdentityRole>().AddEntityFrameworkStores<EcommerceDbContext>();		

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
			//builder.Services.AddScoped<IGenericService<CustomerResponseDTO, CustomerRegistrationDTO>, CustomerService<CustomerRegistrationDTO>>();
			builder.Services.AddScoped<CustomerService<IDto>>();
			builder.Services.AddScoped<AddressService<IDto>>();
			builder.Services.AddScoped<CategoryServices<IDto>>();
			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}