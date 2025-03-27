using Bussines_Logic.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

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

			builder.Services.Configure<JWTSetting>(builder.Configuration.GetSection(nameof(JWTSetting)));

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(//options =>
			//{
			//	options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
			//	{
			//		Name = "Authentication",
			//		Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
			//		Scheme ="Bearer",
			//		BearerFormat = "JWT",
			//		In = Microsoft.OpenApi.Models.ParameterLocation.Header,
			//		Description = "Enter Your Token",
			//	});
			//	options.AddSecurityRequirement(new OpenApiSecurityRequirement
			//	{
			//		{
			//			new OpenApiSecurityScheme
			//			{
			//				Reference = new OpenApiReference
			//				{
			//					Type = ReferenceType.SecurityScheme,
			//					Id = "Bearer"
			//				},
			//				Name = "Bearer",
			//				In = ParameterLocation.Header
			//			},
			//			new List<string>()
			//		}
			//	});
			);

			builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
			builder.Services.AddScoped<IImageService, ImageService>();
			builder.Services.AddScoped<IEmailSender, EmailSender>();
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

			//builder.Services.AddAuthentication(options =>
			//{
			//	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			//	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			//	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			//}).AddJwtBearer(o =>
			//{
			//	o.RequireHttpsMetadata = true;
			//	o.SaveToken = true;
			//	o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
			//	{
			//		ValidateIssuerSigningKey = true,
			//		ValidateIssuer = true,
			//		ValidateAudience = true,
			//		ValidateLifetime = true,
			//		ValidIssuer = builder.Configuration["JWTSetting:Issure"],
			//		ValidAudience = builder.Configuration["JWTSetting:Audience"],
			//		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTSetting:Key"]))
			//	};
			//});
			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseAuthentication();
			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
