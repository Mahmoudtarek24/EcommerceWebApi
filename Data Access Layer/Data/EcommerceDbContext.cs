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

			//ProductImage properity
			modelBuilder.Entity<ProductImage>().HasKey(e => new { e.ImageId});

			//CartItem properity
			modelBuilder.Entity<CartItem>().HasCheckConstraint("CartItemQuentity", "[Quantity] between 0 and 100");

			//Order Properity
			modelBuilder.Entity<Order>().Property(e => e.OrderStatus).HasConversion<string>().IsRequired().HasMaxLength(30);


			foreach (var entitys in modelBuilder.Model.GetEntityTypes())
			{
				//centerlalize string with lenth 100 

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

				//,decimal(12,3)
				var decimalProperitys = entitys.GetProperties().Where(e=>e.ClrType==typeof(decimal) || e.ClrType == typeof(decimal?));
				foreach (var decimalProp in decimalProperitys)
				{
					if (decimalProp.GetPrecision() == null)
					{
						decimalProp.SetPrecision(10);
						decimalProp.SetScale(3);
					}
				}

				//convert delete cascade to Restrict 
				var cascadeFk=entitys.GetForeignKeys().Where(e=>e.DeleteBehavior==DeleteBehavior.Cascade);
				foreach (var fk in cascadeFk)
				{
					fk.DeleteBehavior = DeleteBehavior.Restrict;
				}
			}

			//one-to-one    ApplicationUser-Custome , CartItem-Product  ,Order-BillingAddress
			//  Order-ShippingAddress
			modelBuilder.Entity<Customer>().HasOne(e => e.applicationUser).WithOne().HasForeignKey<Customer>(e => e.UserId);
			modelBuilder.Entity<CartItem>().HasOne(e => e.Product).WithOne().HasForeignKey<CartItem>(e => e.ProductId);
			//modelBuilder.Entity<Order>().HasOne(e => e.BillingAddress).WithOne().HasForeignKey<Order>(e => e.BillingAddressId);
			modelBuilder.Entity<Order>().HasOne(e => e.ShippingAddress).WithOne().HasForeignKey<Order>(e => e.ShippingAddressId);


			//one-to-many  custome-address ,category-product  , Product-ProductImages   , Customer-Order
			//  Order-OrderItem  , Product-OrderItem
			modelBuilder.Entity<Customer>().HasMany(e => e.Addresses).WithOne(e => e.Customer);
			modelBuilder.Entity<Category>().HasMany(e => e.products).WithOne(e => e.Category);
			modelBuilder.Entity<Product>().HasMany(e=>e.productImages).WithOne(e => e.Product);
			modelBuilder.Entity<Cart>().HasMany(e => e.CartItems).WithOne(e => e.Cart);
			modelBuilder.Entity<Customer>().HasMany(e=>e.carts).WithOne(e => e.Customer);
			modelBuilder.Entity<Customer>().HasMany(e => e.orders).WithOne(e => e.customer);
			modelBuilder.Entity<Order>().HasMany(e => e.orderItems).WithOne(e => e.order);
			modelBuilder.Entity<Product>().HasMany(e=>e.orderItems).WithOne(e => e.product);		
		}
		public DbSet<Customer> Customers { get; set; }
		public DbSet<Address> Addresss { get; set; }
		public DbSet<Category> Categories { get; set; }		
		public DbSet<Product> product { get; set; }	
		public DbSet<ProductImage> ProductImage { get; set; }	
		public DbSet<Cart> carts { get; set; }	
		public DbSet<CartItem> cartItems { get; set; }		
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderItem> OrderItems { get; set; }
	}
}
