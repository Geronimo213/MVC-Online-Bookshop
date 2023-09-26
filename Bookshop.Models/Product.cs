using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
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
        public double? Price { get; set; }
    }
}
