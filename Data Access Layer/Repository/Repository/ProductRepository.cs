
namespace Data_Access_Layer.Repository.Repository
{
	public class ProductRepository :GenericRepository<Product>
	{
		private readonly EcommerceDbContext context;
		public ProductRepository(EcommerceDbContext context):base(context) 
		{
			this.context = context;
		}
	}
}
