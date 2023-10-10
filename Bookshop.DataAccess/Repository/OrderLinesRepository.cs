using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
