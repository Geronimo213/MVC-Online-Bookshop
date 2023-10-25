using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookshop.Models
{
    [PrimaryKey(nameof(OrderId), nameof(ProductId))]
    public class OrderLines
    {
        public int Quantity { get; set; }

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }



    }
}
