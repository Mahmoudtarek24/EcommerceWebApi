
namespace Data_Access_Layer.Repository.Repository
{
	public class CustomerRepository : GenericRepository<Customer>
	{
		private readonly EcommerceDbContext context;
		public CustomerRepository(EcommerceDbContext context) : base(context)
		{
		}

	}
}
