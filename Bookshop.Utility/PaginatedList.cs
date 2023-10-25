using Bookshop.Models;
using Bookshop.Models.ViewModels;
using Microsoft.EntityFrameworkCore;


namespace Bookshop.Utility
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public PaginatedList(int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            pageIndex = Math.Min(pageIndex, (int)Math.Ceiling(count / (double)pageSize));
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
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
                if (!String.IsNullOrEmpty(includeOperators))
                {
                    foreach (var includeOperator in includeOperators.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    {
                        lineQuery = lineQuery.Include(includeOperator);
                    }
                }
                orderVM.Lines = lineQuery;
                orderList.Add(orderVM);
            }

            return new PaginatedList<OrderVM>(orderList, count, pageIndex, pageSize);
        }
    }


}