using Bussines_Logic.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
			builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
				.AddEntityFrameworkStores<EcommerceDbContext>().AddDefaultTokenProviders(); 
			builder.Services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));		

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
			builder.Services.AddScoped<IImageService, ImageService>();
			builder.Services.AddScoped<IEmailSender,EmailSender>();
			builder.Services.AddScoped<CustomerService<IDto>>();
			builder.Services.AddScoped<AddressService<IDto>>();
			builder.Services.AddScoped<CategoryServices<IDto>>();
			builder.Services.AddScoped<ProductServices<IDto>>();
			builder.Services.AddScoped<OrderServices<IDto>>();
			builder.Services.AddScoped<ShoppingCartService>();
			builder.Services.AddScoped<PaymentServices>();
			builder.Services.AddScoped<CancellationServices>();
			builder.Services.AddScoped<FeedBackServices>();
			builder.Services.AddScoped<AdminServices>();
			builder.Services.AddSingleton<EmailVerification>();
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