using Bookshop.DataAccess.Data;
using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;

namespace Bookshop.DataAccess.Repository
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private AppDBContext DbContext { get; set; }
        public OrderRepository(AppDBContext appDbContext) : base(appDbContext)
        {
            this.DbContext = appDbContext;
        }

        public void Update(Order order)
        {
            DbContext.Orders.Update(order);
        }

        public void UpdateStatus(int id, string orderStatus)
        {
            var orderDb = DbContext.Orders.FirstOrDefault(x => x.OrderId == id);

            if (orderDb == null) return;
            if (!string.IsNullOrEmpty(orderStatus))
            {
                orderDb.OrderStatus = orderStatus;
            }
        }

        public void UpdateStripe(int id, string sessionId, string paymentId)
        {
            var orderDb = DbContext.Orders.FirstOrDefault(x => x.OrderId == id);

            if (orderDb == null) return;
            if (!string.IsNullOrEmpty(sessionId))
            {
                orderDb.SessionId = sessionId;
            }

            if (!string.IsNullOrEmpty(paymentId))
            {
                orderDb.PaymentIntentId = paymentId;
            }
        }
    }
}
