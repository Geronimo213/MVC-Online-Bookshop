using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bookshop.Utility;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookshop.Models.ViewModels
{
    public class PaginatedUserVM
    {

        public PaginatedList<AppUser> Users { get; set; }
        public IEnumerable<SelectListItem> RoleList { get; set; } = Enumerable.Empty<SelectListItem>();

        public List<string> SelectedRoles { get; set; } = new List<string>();
    }
}
