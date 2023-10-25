using Bookshop.DataAccess.Data;
using Bookshop.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Bookshop.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        internal DbSet<T> DbSet { get; init; }

        public Repository(AppDBContext dbContext)
        {
            this.DbSet = dbContext.Set<T>();
        }


        public void Add(T entity)
        {
            DbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            DbSet.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            DbSet.RemoveRange(entities);
        }

        public async Task<T?> Get(Expression<Func<T, bool>> filter, string? includeOperators = null, bool tracked = true)
        {
            IQueryable<T> query = tracked ? DbSet : DbSet.AsNoTracking();
            query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeOperators))
            {
                query = includeOperators.Split(',', StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeOperator) => current.Include(includeOperator));
            }
            return await query.FirstOrDefaultAsync();
        }

        public IQueryable<T> GetAll(string? includeOperators = null)
        {
            IQueryable<T> query = DbSet;
            if (!string.IsNullOrEmpty(includeOperators))
            {
                query = includeOperators.Split(',', StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeOperator) => current.Include(includeOperator));
            }
            return query;
        }

        public IQueryable<T>? GetAll(Expression<Func<T, bool>> filter, string? includeOperators = null)
        {
            IQueryable<T> query = DbSet;
            query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeOperators))
            {
                query = includeOperators.Split(',', StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeOperator) => current.Include(includeOperator));
            }
            return query;
        }
    }
}
