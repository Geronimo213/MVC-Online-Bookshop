using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MVC_Online_Bookshop.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "A name for the category is required.")]
        [DisplayName("Category Name")]
        [MaxLength(30, ErrorMessage = "Category name must be 30 characters or less.")]
        [NotNull]
        public string? Name { get; set; }

        [DisplayName("Display Order")]
        [Range(1, 100, ErrorMessage = "Order must be between 1 and 100.")]
        public int DisplayOrder { get; set; }
    }
}
