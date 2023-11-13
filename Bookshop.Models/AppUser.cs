using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookshop.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        public string Name { get; set; } = "Unknown";

        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        [RegularExpression(@"^\d{5}(?:[-\s]\d{4})?$", ErrorMessage = "Not a valid zip code")]
        public string? PostalCode { get; set; }

        [NotMapped]
        public string? Role { get; set; }
    }
}
