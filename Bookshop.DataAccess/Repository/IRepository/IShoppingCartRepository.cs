using Bookshop.Models;

namespace Bookshop.DataAccess.Repository.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        void Update(ShoppingCart shoppingCart);
        Task MigrateSessionCart(string sessionId, AppUser user);
    }
}
