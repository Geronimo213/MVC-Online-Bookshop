using Bookshop.Models;

namespace Bookshop.DataAccess.Repository.IRepository
{
    public interface IOrderRepository : IRepository<Order>
    {
        void Update(Order order);

        void UpdateStatus(int id, string orderStatus);

        void UpdateStripe(int id, string sessionId, string paymentId);
    }
}
