using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repository.Repository
{
	public class CartItemRepository :GenericRepository<CartItem> 
	{
		private readonly EcommerceDbContext context;
		public CartItemRepository(EcommerceDbContext context):base(context) 
		{
				this.context = context;	
		}
	}
}
