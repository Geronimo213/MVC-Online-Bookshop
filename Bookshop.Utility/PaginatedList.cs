using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
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
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
        public static async Task<PaginatedList<OrderVM>> CreateAsync(IQueryable<Order> source, IQueryable<OrderLines> joinSource, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            var orderList = new List<OrderVM>();
            foreach (var item in items)
            {
                var orderVM = new OrderVM
                {
                    Header = item,
                    Lines = await joinSource.Where(x => x.OrderId == item.OrderId).ToListAsync()
                };
                orderList.Add(orderVM);
            }

            return new PaginatedList<OrderVM>(orderList, count, pageIndex, pageSize);
        }
    }


}