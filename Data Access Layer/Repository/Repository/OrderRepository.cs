
namespace Data_Access_Layer.Repository.Repository
{
	public class OrderRepository :GenericRepository<Order>
	{
		private readonly EcommerceDbContext context;
		public OrderRepository(EcommerceDbContext context) :base(context)
		{
			this.context = context;
		}

		public async Task<IEnumerable<Order>> OrdersData()
		{
			var Orders=context.Orders.AsNoTracking().Include(e=>e.BillingAddress).Include(e=>e.ShippingAddress)
				                     .Include(e=>e.orderItems).ThenInclude(e => e.product)
									 .Include(e=>e.customer).ToList();	

			return Orders;
		}

		public async Task<IEnumerable<Order>?> OrdersForCustome(int customerId)
		{
			var Orders = context.Orders.AsNoTracking().Include(e => e.BillingAddress).Include(e => e.ShippingAddress)
									 .Include(e => e.orderItems).ThenInclude(e => e.product)
									 .Include(e => e.customer).Where(e=>e.CustomerId== customerId).ToList();

			return Orders;
		}

		public async Task<Order?> OrdersEmailDate(int orderId)
		{
			var Orders =await context.Orders.AsNoTracking().Include(e => e.BillingAddress).Include(e => e.ShippingAddress)
									 .Include(e => e.orderItems).ThenInclude(e => e.product).Include(e => e.payment)
									 .Include(e => e.customer).ThenInclude(e=>e.applicationUser)
									 .SingleOrDefaultAsync(e=>e.OrderId==orderId);

			return  Orders;
		}

		public async Task<Order?> CancellationOrders(Expression<Func<Order,bool>> Filter)
		{
			var Orders = await context.Orders.Include(e => e.orderItems)
				                      .ThenInclude(e => e.product)
									 .Include(e => e.customer).ThenInclude(e => e.applicationUser)
									 .SingleOrDefaultAsync(Filter);

			return Orders;
		}

	}
}
