using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookshop.Models
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }
        public double Total { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product Product { get; set; }

        [Range(1, 5, ErrorMessage = "Limit 5 per person.")]
        public int Count { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        [ValidateNever]
        public AppUser? User { get; set; }

    }
}
