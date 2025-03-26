using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Data_Access_Layer.Repository.IRepository
{
	public interface IGenericRepository <T> where T : class
	{
	    Task<IEnumerable<T>> GetAllEntitiesAsync(Expression<Func<T, bool>> Filter = null, string[]Includes =null, bool track=false);
		Task<T?> GetEntityAsync(Expression<Func<T, bool>> filter, string[] Includes = null, bool tracked = false);
		Task<bool> GetAnyEntityAsync(Expression<Func<T, bool>> filter, string[] Includes = null, bool tracked = false);
		Task<T> GetByIdAsync(int id);
		Task Insert(T entity);
		Task Delete(T entity);	
	}
}
