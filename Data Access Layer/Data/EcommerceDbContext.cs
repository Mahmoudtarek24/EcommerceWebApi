using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Data_Access_Layer.Data
{
	public class EcommerceDbContext :IdentityDbContext<ApplicationUser>
	{
		private readonly List<string> IdentityTables = new List<string> { "AspNetRoles", "AspNetRoleClaims",
								"AspNetUserClaims", "AspNetUserLogins",
								"AspNetUserRoles", "AspNetUserTokens", "AspNetUsers" };
		public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options):base(options) 
		{

		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			//unique Value for   Application( username ,email)

			modelBuilder.Entity<ApplicationUser>().HasIndex(e => e.Email).HasFilter(null);	
			modelBuilder.Entity<ApplicationUser>().HasIndex(e => e.UserName).HasFilter(null);

			//unique Value for  Category CategoryName
			modelBuilder.Entity<Category>().HasIndex(e=>e.Name).HasFilter(null);

			//Product properity
			modelBuilder.Entity<Product>().HasCheckConstraint("DiscountPercentage", "[DiscountPercentage] between 0 and 100");
			modelBuilder.Entity<Product>().HasCheckConstraint("Price", "[Price]>= 0 ");
			modelBuilder.Entity<Product>().HasCheckConstraint("StockQuantity", "[StockQuantity]>= 0 ");
			modelBuilder.Entity<Product>().Property(e => e.Price).HasPrecision(12, 3);


			//centerlalize string with lenth 100 
			foreach (var entitys in modelBuilder.Model.GetEntityTypes())
			{
				string tableName = entitys.GetTableName();

				if (!IdentityTables.Contains(tableName))
				{
					foreach(var StringProperity in entitys.GetProperties().Where(e=>e.ClrType==typeof(string)))
					{
						if (StringProperity.GetMaxLength() == null)
						{
							StringProperity.SetMaxLength(50);	
						}
					}
				}
			}

			//one-to-one    ApplicationUser-Custome
			modelBuilder.Entity<Customer>().HasOne(e => e.applicationUser).WithOne().HasForeignKey<Customer>(e => e.UserId);

			//one-to-many  custome-address ,category-product
			modelBuilder.Entity<Customer>().HasMany(e => e.Addresses).WithOne(e => e.Customer);
			modelBuilder.Entity<Category>().HasMany(e => e.products).WithOne(e => e.Category);
		}
		public DbSet<Customer> Customers { get; set; }
		public DbSet<Address> Addresss { get; set; }
		public DbSet<Category> Categories { get; set; }		
	}
}
