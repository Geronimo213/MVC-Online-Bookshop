using System.ComponentModel.DataAnnotations;

namespace Bookshop.Models
{
    public class Header
    {

        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        [Required]
        public string AltText { get; set; } = string.Empty;

        public string? LinkPath { get; set; }

        [Required]
        [Range(1, 25)]
        public int DisplayOrder { get; set; }

    }
}
