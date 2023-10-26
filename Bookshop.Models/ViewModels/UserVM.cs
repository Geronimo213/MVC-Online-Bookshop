using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookshop.Models.ViewModels
{
    public class UserVM
    {
        public AppUser User { get; set; } = new AppUser();
        public IEnumerable<SelectListItem> RoleList { get; set; } = Enumerable.Empty<SelectListItem>();

        public List<string> SelectedRoles { get; set; } = new List<string>();

    }
}
