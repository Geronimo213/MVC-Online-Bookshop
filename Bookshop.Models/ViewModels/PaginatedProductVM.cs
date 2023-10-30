using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bookshop.Utility;

namespace Bookshop.Models.ViewModels
{
    public class PaginatedProductVM
    {
        public readonly List<string> SortParams = new (){ "Title", "Author" };
        public PaginatedList<Product> Products { get; set; } = new PaginatedList<Product>(0, 1, 1);
        public string? SortOrder { get; set; }
        public string? SearchParam { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
