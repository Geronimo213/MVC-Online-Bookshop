using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookshop.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "A title is required for each book.")]
        [NotNull]
        public string? Title { get; set; }
        [Required(ErrorMessage = "Description is required.")]
        [NotNull]
        public string? Description { get; set; }
        [Required(ErrorMessage = "ISBN required.")]
        public string? ISBN { get; set; }
        [Required(ErrorMessage = "Author is required.")]
        public string? Author { get; set; }
        [Range(1, 1000)]
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid price. Price must be between $1.00 and $1000.00 and rounded to two (2) decimal places.")]
        public double? Price { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
        public string ImageURL { get; set; }
    }
}
