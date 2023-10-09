using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookshop.Models.ViewModels
{
    public class CheckoutVM
    {
        public IEnumerable<ShoppingCart> Items { get; set; }

        public Order Order { get; set; }
    }
}
