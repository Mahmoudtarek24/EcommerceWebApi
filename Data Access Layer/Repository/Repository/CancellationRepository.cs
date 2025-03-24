using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repository.Repository
{
	public class CancellationRepository :GenericRepository<Cancellation>
	{
		private readonly EcommerceDbContext context;
		public CancellationRepository(EcommerceDbContext context):base(context) 
		{
			this.context = context;
		}
		public async Task<Cancellation?> CancellationOrders(Expression<Func<Cancellation, bool>> Filter)
		{
			var Orders = await context.cancellations.Include(e=>e.Order).ThenInclude(e=>e.customer)
				                         .ThenInclude(e=>e.applicationUser)
										 .Include(e => e.Order)
									  .ThenInclude(e => e.orderItems)
									 .ThenInclude(e=>e.product)
									 .SingleOrDefaultAsync(Filter);

			return Orders;
		}
	}
}
