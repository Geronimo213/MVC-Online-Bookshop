using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bookshop.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        //For Category type
        IQueryable<T> GetAll(string? includeOperators = null);
        T? Get(Expression<Func<T, bool>> filter, string? includeOperators = null);
        void Add(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
    }
}
