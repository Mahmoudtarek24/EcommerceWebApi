
namespace Data_Access_Layer.Repository.Repository
{
	public class AddressRepository : GenericRepository<Address>
	{
		private readonly EcommerceDbContext context;
		public AddressRepository(EcommerceDbContext context) : base(context)
		{
			this.context = context;
		}
		public async Task<IEnumerable<Address>> GetAddressesByCustomer(Expression<Func<Address,bool>> Filter, string[] Includes =null) {
			
			IQueryable<Address> query= context.Addresss.AsQueryable();

			if (Includes != null)
			{
				foreach(var Include in Includes)
					query= query.Include(Include);
			}
			return  await query.Where(Filter).ToListAsync();	
		}
	}
}
