
namespace Data_Access_Layer.Repository.Repository
{
	public class OrderRepository :GenericRepository<Order>
	{
		private readonly EcommerceDbContext context;
		public OrderRepository(EcommerceDbContext context) :base(context)
		{
			this.context = context;
		}
	}
}
