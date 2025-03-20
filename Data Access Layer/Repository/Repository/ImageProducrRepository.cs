
namespace Data_Access_Layer.Repository.Repository
{
	public class ImageProducrRepository :GenericRepository<ProductImage>
	{
		private readonly EcommerceDbContext context;
		public ImageProducrRepository(EcommerceDbContext context):base(context) 
		{
			this.context = context;	
		}
	}
}
