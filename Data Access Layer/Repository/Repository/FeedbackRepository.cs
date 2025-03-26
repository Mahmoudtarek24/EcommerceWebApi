
namespace Data_Access_Layer.Repository.Repository
{
	public class FeedbackRepository :GenericRepository<Feedback>
	{
		private readonly EcommerceDbContext context;
		public FeedbackRepository(EcommerceDbContext context):base(context) 
		{
			this.context = context;
		}

		public Feedback? FeedBackInformation(Expression<Func<Feedback, bool>> Filter)
		{
			return  context.feedbacks.Include(e => e.Customer).ThenInclude(e => e.applicationUser)
								.Include(e => e.Product).FirstOrDefault(Filter);
				 
		}
	}
}
