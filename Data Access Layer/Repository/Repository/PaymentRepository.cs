
namespace Data_Access_Layer.Repository.Repository
{
	public class PaymentRepository  :GenericRepository<Payment>
	{
		private readonly EcommerceDbContext context;
		public PaymentRepository(EcommerceDbContext context):base(context) 
		{
			this.context = context;
		}

	}
}
