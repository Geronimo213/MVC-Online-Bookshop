
using Microsoft.EntityFrameworkCore;


namespace Bookshop.Utility
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageIndex = pageIndex;

            this.AddRange(items);
        }

        public PaginatedList(int count, int pageIndex, int pageSize)
        {
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageIndex = pageIndex;
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

    }


}