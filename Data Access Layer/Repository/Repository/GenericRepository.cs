
namespace Data_Access_Layer.Repository.Repository
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		private readonly EcommerceDbContext context;
		private DbSet<T> Table { get; set; }	
		public GenericRepository(EcommerceDbContext context)
		{
			this.context = context;
			Table=context.Set<T>();
		}

		public async Task Delete(T entity)
		{
		    Table.Remove(entity);	
		}

		public async Task<IEnumerable<T>> GetAllEntitiesAsync(Expression<Func<T, bool>> Filter = null, string[] Includes = null, bool track =false)
		{
			IQueryable<T> query = Table.AsQueryable();

			if (track)
			{
				query = query.AsNoTracking();	
			}
			if (Includes != null)
			{
				foreach(var Include in Includes)
					query=query.Include(Include);	
			}
			if(Filter !=null)
			{
				query=query.Where(Filter);
			}

			return await query.ToListAsync();
		}

		public async Task<T?> GetEntityAsync(Expression<Func<T, bool>> filter, string[] Includes = null, bool tracked = false)
		{
			IQueryable<T> query = Table.AsQueryable();
			if(tracked)
				query=query.AsNoTracking();
			
			if (Includes != null)
			{
				foreach (var Include in Includes)
					query = query.Include(Include);
			}
			
			return await query.SingleOrDefaultAsync(filter);	

		}
		public async Task<T> GetByIdAsync(int id)
		{
			return await Table.FindAsync(id);
		}

		public async Task Insert(T entity)
		{
		  await Table.AddAsync(entity);	
		}
	}
}
