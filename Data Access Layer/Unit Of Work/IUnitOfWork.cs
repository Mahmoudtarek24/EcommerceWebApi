
namespace Data_Access_Layer.Unit_Of_Work
{
	public interface IUnitOfWork : IDisposable	
	{
		CustomerRepository CustomerRepository { get; }	
		AddressRepository AddressRepository { get; }
		CategoryRepository CategoryRepository { get; }
		ProductRepository ProductRepository { get; }
		ImageProducrRepository imageProducrRepository { get; }	
		CartItemRepository CartItemRepository { get; }
		CartRepository CartRepository { get; }
		OrderItemRepository OrderItemRepository { get; }
		OrderRepository OrderRepository { get; }
		PaymentRepository PaymentRepository { get; }
		CancellationRepository cancellationRepository { get; }
		RefundRepository RefundRepository { get; }
		FeedbackRepository FeedbackRepository { get; }
		AdminRepository AdminRepository { get; }	
		Task Commit();
		Task RollBack();
		Task Save();
		Task CreateTarsaction();

	}
}
