using Bookshop.Utility;
using Microsoft.EntityFrameworkCore;

namespace Bookshop.Models.ViewModels
{
    public class OrderVM
    {
        public Order Header { get; set; } = new Order();
        public IEnumerable<OrderLines> Lines { get; set; } = Enumerable.Empty<OrderLines>();


    }

    public class PaginatedOrders : PaginatedList<OrderVM>
    {
        public PaginatedOrders(int count, int pageIndex, int pageSize) : base(count, pageIndex, pageSize)
        {

        }
        public static async Task<PaginatedList<OrderVM>> CreateAsync(IQueryable<Order> source, IQueryable<OrderLines> joinSource, int pageIndex, int pageSize, string? includeOperators)
        {
            var count = await source.CountAsync();
            pageIndex = Math.Min(pageIndex, (int)Math.Ceiling(count / (double)pageSize));
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            var orders = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            var orderList = new List<OrderVM>();
            foreach (var order in orders)
            {
                var orderVM = new OrderVM
                {
                    Header = order
                };
                var lineQuery = joinSource.Where(x => x.OrderId == order.OrderId);
                if (!string.IsNullOrEmpty(includeOperators))
                {
                    lineQuery = includeOperators.Split(',', StringSplitOptions.RemoveEmptyEntries).Aggregate(lineQuery, (current, includeOperator) => current.Include(includeOperator));
                }
                orderVM.Lines = lineQuery;
                orderList.Add(orderVM);
            }

            return new PaginatedList<OrderVM>(orderList, count, pageIndex, pageSize);
        }
    }
}
