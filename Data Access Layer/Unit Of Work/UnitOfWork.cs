
namespace Data_Access_Layer.Unit_Of_Work
{
	public class UnitOfWork : IUnitOfWork
	{
		public CustomerRepository CustomerRepository { get; }
		public AddressRepository AddressRepository { get; }
		public CategoryRepository CategoryRepository { get; }
		public ProductRepository ProductRepository { get; }
		public ImageProducrRepository imageProducrRepository { get; }
		public CartItemRepository CartItemRepository { get; }	
		public CartRepository CartRepository { get; }
		public OrderItemRepository OrderItemRepository { get; }
		public OrderRepository OrderRepository { get; }
		public PaymentRepository PaymentRepository { get; }
		public CancellationRepository cancellationRepository { get; }
		public RefundRepository RefundRepository { get; }
		public FeedbackRepository FeedbackRepository { get; }
		public AdminRepository AdminRepository { get; }	

		private readonly EcommerceDbContext context;
		private IDbContextTransaction transaction;
		public UnitOfWork(EcommerceDbContext context)
		{
			this.context = context;
			CustomerRepository = new CustomerRepository(context);
			AddressRepository= new AddressRepository(context);	
			CategoryRepository= new CategoryRepository(context);
			ProductRepository = new ProductRepository(context);	
			imageProducrRepository= new ImageProducrRepository(context);	
		    CartRepository= new CartRepository(context);	
			CartItemRepository= new CartItemRepository(context);	
			OrderItemRepository= new OrderItemRepository(context);	
			OrderRepository= new OrderRepository(context);	
			PaymentRepository= new PaymentRepository(context);
		    cancellationRepository =new CancellationRepository(context);
			RefundRepository= new RefundRepository(context);	
			FeedbackRepository =new FeedbackRepository(context);	
			AdminRepository=new AdminRepository(context);	
		}

		public async Task Commit()
		{
			await transaction.CommitAsync();	
		}

		public async Task CreateTarsaction()
		{
			transaction = await context.Database.BeginTransactionAsync();	
		}

		public async Task RollBack()
		{
			await transaction.RollbackAsync();	
		}

		public async  Task Save()
		{
		   await context.SaveChangesAsync();
		}

		public void Dispose()
		{
			context.Dispose();	
		}
	}
}
