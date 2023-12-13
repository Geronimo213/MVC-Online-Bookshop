using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Bookshop.Models.ViewModels
{
    public class CheckoutVM
    {
        public IEnumerable<ShoppingCart> Items { get; set; } = Enumerable.Empty<ShoppingCart>();

        public Order Order { get; set; } = new();
        public string SessionId { get; set; }

        [ValidateNever]
        public List<int>? ItemIds { get; set; }
    }
}
