using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookshop.Models
{
    public class BookList
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please give the list a name.")]
        [StringLength(25, ErrorMessage = "25 characters max.")]
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<Product> Books { get; set; } = new List<Product>();
    }
}
