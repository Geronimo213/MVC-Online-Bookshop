using Bookshop.Models;

namespace Bookshop.DataAccess.Repository.IRepository
{
    public interface IOrderLinesRepository : IRepository<OrderLines>
    {
        void Update(OrderLines orderLine);
    }
}
