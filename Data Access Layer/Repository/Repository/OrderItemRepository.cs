
namespace Data_Access_Layer.Repository.Repository
{
	public  class OrderItemRepository :GenericRepository<OrderItem>
	{
		private readonly EcommerceDbContext context;
		public OrderItemRepository(EcommerceDbContext context) :base(context) 
		{
			this.context = context;
		}
	}
}
