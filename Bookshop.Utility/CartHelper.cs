using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Bookshop.Utility
{
    /// <summary>
    /// Helper functions for handling cart functions across controllers
    /// </summary>
    public static class CartHelper
    {
        /// <summary>
        /// Gets the current user's shopping cart cookie if available. Otherwise, creates a new SessionId and hands out the cookie.
        /// </summary>
        /// <param name="ctx">HttpContext of the calling controller.</param>
        /// <returns>A string containing SessionId found/stored within the user cookie.</returns>
        public  static string GetCartCookie(HttpContext ctx)
        {
            //Access passed context and try to get cookie. If cookie returns null, set to empty string.
            var cartCookie = ctx.Request.Cookies[SD.ShoppingCartCookie] ?? string.Empty;

            //Check for early return. Else, create a new guid and hand out a cookie. Ultimately, return
            //the newly created SessionId to caller.
            if (!string.IsNullOrEmpty(cartCookie)) return cartCookie;
            cartCookie = Guid.NewGuid().ToString();
            var options = new CookieOptions()
            {
                Expires = DateTime.Now.AddMonths(9),
                IsEssential = true,
                HttpOnly = true
            };
            ctx.Response.Cookies.Append(SD.ShoppingCartCookie, cartCookie, options);

            return cartCookie;
        }

    }
}
