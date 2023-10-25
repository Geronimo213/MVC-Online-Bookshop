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
    }
}
