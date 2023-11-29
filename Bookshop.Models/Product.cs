using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

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
        [StringLength(17, MinimumLength = 10, ErrorMessage = "ISBN must be between 10 and 17 characters and may include hyphens (-).")]
        public string? ISBN { get; set; }
        [Required(ErrorMessage = "Author is required.")]
        public string? Author { get; set; }
        [Range(1, 1000)]
        [RegularExpression(@"(([1-9]\d{0,2}(,\d{3})*)|(([1-9]\d*)?\d))(\.\d\d)?$", ErrorMessage = "Invalid price. Price must be between $1.00 and $1000.00 and rounded to two (2) decimal places.")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal? Price { get; set; }
        public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();
        [AllowNull]
        public string? ImageURL { get; set; }

        public virtual ICollection<BookList> BookLists { get; set; }
    }
}