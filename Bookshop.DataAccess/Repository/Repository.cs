using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Bookshop.DataAccess.Data;
using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using Microsoft.EntityFrameworkCore;

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

        public T Get(Expression<Func<T, bool>> filter, string? includeOperators = null)
        {
            IQueryable<T> query = DbSet;
            query = query.Where(filter);
            if (!String.IsNullOrEmpty(includeOperators))
            {
                foreach (var includeOperator in includeOperators.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeOperator);
                }
            }
            return query.FirstOrDefault();
        }

        public IQueryable<T> GetAll(string? includeOperators = null)
        {
            IQueryable<T> query = DbSet;
            if (!String.IsNullOrEmpty(includeOperators))
            {
                foreach (var includeOperator in includeOperators.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeOperator);
                }
            }
            return query;
        }
    }
}
