
using Data_Access_Layer.Repository.Repository;

namespace Data_Access_Layer.Unit_Of_Work
{
	public interface IUnitOfWork
	{
		CustomerRepository CustomerRepository { get; }	
		AddressRepository AddressRepository { get; }
		CategoryRepository CategoryRepository { get; }
		ProductRepository ProductRepository { get; }	
		Task Commit();
		Task RollBack();
		Task Save();
		Task CreateTarsaction();

	}
}
