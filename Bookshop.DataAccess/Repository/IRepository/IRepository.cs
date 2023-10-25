using System.Linq.Expressions;

namespace Bookshop.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        //For Category type
        IQueryable<T> GetAll(string? includeOperators = null);
        IQueryable<T>? GetAll(Expression<Func<T, bool>> filter, string? includeOperators = null);
        Task<T?> Get(Expression<Func<T, bool>> filter, string? includeOperators = null, bool tracked = true);
        void Add(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
    }
}
