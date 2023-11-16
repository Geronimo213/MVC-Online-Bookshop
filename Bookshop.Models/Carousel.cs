using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookshop.Models
{
    public class Carousel
    {
        public int Id { get; set; }
        [Required]
        [RegularExpression(@"^((?!MISSING_NAME).)*$", ErrorMessage = "Please change the title from default value.")]
        public string Title { get; set; } = "MISSING_NAME";

        [DisplayName("Sort Order")]
        public string SortOrder { get; set; } = "Id";

        [Required(ErrorMessage = "Please choose a display order.")]
        [DisplayName("Display Order")]
        [Range(1, 25, ErrorMessage = "Must be a number between 1 and 25")]
        public int DisplayOrder { get; set; }


        [Required(ErrorMessage = "Please choose a category/tag")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        [ValidateNever]
        public virtual Category Category { get; set; }
    }
}
