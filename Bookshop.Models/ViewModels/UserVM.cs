using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookshop.Models.ViewModels
{
    public class UserVM
    {
        public AppUser User { get; set; } = new AppUser();
        public IEnumerable<SelectListItem> RoleList { get; set; } = Enumerable.Empty<SelectListItem>();

    }
}
