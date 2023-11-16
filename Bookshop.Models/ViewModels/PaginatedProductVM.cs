using Bookshop.Utility;

namespace Bookshop.Models.ViewModels
{
    public class PaginatedProductVM
    {
        public readonly List<string> SortParams = new() { "Title", "Author" };
        public PaginatedList<Product> Products { get; set; } = new PaginatedList<Product>(0, 1, 1);
        public string? SortOrder { get; set; }
        public string? CurrentFilter { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
