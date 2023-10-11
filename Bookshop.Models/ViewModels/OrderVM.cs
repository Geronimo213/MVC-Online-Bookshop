using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Bookshop.Models.ViewModels
{
    public class OrderVM
    {
        public Order Header { get; set; }
        public IEnumerable<OrderLines> Lines { get; set; }

        public OrderVM()
        {
            this.Header = new Order();
            this.Lines = new List<OrderLines>();
        }
    }
}
