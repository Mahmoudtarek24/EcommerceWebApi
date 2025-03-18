
namespace Data_Access_Layer.Repository.Repository
{
	public class CategoryRepository :GenericRepository<Category>
	{
		private readonly EcommerceDbContext context;
		public CategoryRepository(EcommerceDbContext context) : base(context)
		{
			this.context = context;
		}
	}

}
