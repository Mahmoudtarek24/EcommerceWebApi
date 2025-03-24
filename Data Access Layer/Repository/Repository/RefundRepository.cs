
namespace Data_Access_Layer.Repository.Repository
{
	public class RefundRepository :GenericRepository<Refund>
	{
		private readonly EcommerceDbContext context;
		public RefundRepository(EcommerceDbContext context):base(context) 
		{
			this.context = context;
		}
	}
}
