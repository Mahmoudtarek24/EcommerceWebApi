using Microsoft.EntityFrameworkCore;
using System;

namespace Data_Access_Layer.Repository.Repository
{
	public class CartRepository :GenericRepository<Cart>
	{
		private readonly EcommerceDbContext context;
		public CartRepository(EcommerceDbContext context):base(context)
		{
			this.context = context;
		}

		public async Task<Cart?> RetriveActiveCart(int customerId)
		{
			return await context.carts
					.Include(c => c.CartItems)
						.ThenInclude(ci => ci.Product)
					.FirstOrDefaultAsync(c => c.customerId == customerId && !c.IsCheckedOut);
		}
		public async Task<Cart?> RetriveLastUpdateOrAddedCartItemAndProduct(int CartId)
		{
			return await context.carts.Include(c => c.CartItems)
				                .ThenInclude(e=>e.Product).FirstOrDefaultAsync(e=>e.cartId==CartId)??new Cart();
		}
	}
}
