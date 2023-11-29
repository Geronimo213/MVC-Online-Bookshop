using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Bookshop.Models.ViewModels
{
    public class BookListVM
    {
        [ValidateNever]
        public BookList List { get; set; } = new BookList();

        [Required(ErrorMessage = "Could not find book id. Please try again or contact admin.")]
        public int NewBookId { get; set; }
    }
}
