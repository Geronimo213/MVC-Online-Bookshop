using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookshop.Models.ViewModels
{
    public class FrontPageVM
    {

        public List<Carousel> Carousels { get; set; } = new List<Carousel>();

        public string Title { get; set; } = "Welcome to Ye Olde Bookshop!";

    }
}
