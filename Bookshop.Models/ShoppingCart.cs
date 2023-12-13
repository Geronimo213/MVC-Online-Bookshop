using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Bookshop.Models
{
    [Index(nameof(SessionId))]
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }

      [ForeignKey("Product")]
        public int ProductId { get; set; }
  
        [ValidateNever]
        public Product Product { get; set; }

        [Range(1, 5, ErrorMessage = "Limit 5 per person.")]
        public int Count { get; set; }
        [ForeignKey("User")]
        public string? UserId { get; set; }
        [ValidateNever]
        public AppUser? User { get; set; }
        [Required]
        public string SessionId { get; set; }
    }
}
