using Data_Access_Layer.Repository.Repository;
using Microsoft.EntityFrameworkCore.Storage;

namespace Data_Access_Layer.Unit_Of_Work
{
	public class UnitOfWork : IUnitOfWork
	{
		public CustomerRepository CustomerRepository { get; }

		public AddressRepository AddressRepository { get; }
		public CategoryRepository CategoryRepository { get; }

		public ProductRepository ProductRepository { get; }	

		private readonly EcommerceDbContext context;
		private IDbContextTransaction transaction;
		public UnitOfWork(EcommerceDbContext context)
		{
			this.context = context;
			CustomerRepository = new CustomerRepository(context);
			AddressRepository= new AddressRepository(context);	
			CategoryRepository= new CategoryRepository(context);	
			ProductRepository= new ProductRepository(context);	
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
	}
}
