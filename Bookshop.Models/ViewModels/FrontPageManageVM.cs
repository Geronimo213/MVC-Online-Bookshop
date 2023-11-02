using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookshop.Models.ViewModels
{
    public class FrontPageManageVM
    {
        public List<Carousel> Carousels { get; set; } = new();
        public List<Header> Headers { get; set; } = new();
    }
}
