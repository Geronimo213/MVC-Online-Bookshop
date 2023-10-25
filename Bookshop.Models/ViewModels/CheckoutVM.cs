namespace Bookshop.Models.ViewModels
{
    public class CheckoutVM
    {
        public IEnumerable<ShoppingCart> Items { get; set; } = Enumerable.Empty<ShoppingCart>();

        public Order Order { get; set; } = new Order();
    }
}
