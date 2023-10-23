using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Bookshop.Models
{
    public class AppUser : IdentityUser
    {
        [Required] 
        public string Name { get; set; } = "Unknown";

        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set;}

        [NotMapped]
        public string? Role { get; set; }
    }
}
