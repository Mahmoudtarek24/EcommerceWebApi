﻿
namespace Data_Access_Layer.Unit_Of_Work
{
	public interface IUnitOfWork
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
		Task Commit();
		Task RollBack();
		Task Save();
		Task CreateTarsaction();

	}
}
