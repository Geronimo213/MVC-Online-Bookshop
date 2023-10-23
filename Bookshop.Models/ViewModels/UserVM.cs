using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Bookshop.Models.ViewModels
{
    public class UserVM
    {
        public AppUser User { get; set; }
        public IdentityRole Role { get; set; }
    }
}
