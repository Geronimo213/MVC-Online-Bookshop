using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bookshop.Models;
using Microsoft.AspNetCore.Identity;

namespace Bookshop.Utility
{
    public static class UserManagerExtensions
    {
        public static Task<string?> GetUserAddressAsync(this UserManager<AppUser> userManager, AppUser user)
        {
            return Task.FromResult(user.StreetAddress);
        }
    }
}
