using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookshop.Models.ViewModels
{
    public class ProductVM()
    {
        public Product Product { get; set; } = new Product();
        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
