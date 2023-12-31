﻿using Bookshop.Utility;
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
