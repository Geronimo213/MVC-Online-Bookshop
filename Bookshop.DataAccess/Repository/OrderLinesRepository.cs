using Bookshop.DataAccess.Data;
using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;

namespace Bookshop.DataAccess.Repository
{
    public class OrderLinesRepository : Repository<OrderLines>, IOrderLinesRepository
    {
        private AppDBContext dbContext { get; }

        public OrderLinesRepository(AppDBContext appDbContext) : base(appDbContext)
        {
            this.dbContext = appDbContext;
        }

        public void Update(OrderLines orderLine)
        {
            dbContext.OrderLines.Update(orderLine);
        }
    }
}
