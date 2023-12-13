using Bookshop.DataAccess.Data;
using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookshop.DataAccess.Repository
{
    internal class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly AppDBContext _appDbContext;
        public ShoppingCartRepository(AppDBContext dbContext) : base(dbContext)
        {
            this._appDbContext = dbContext;
        }

        public void Update(ShoppingCart shoppingCart)
        {
            _appDbContext.ShoppingCarts.Update(shoppingCart);
        }

        public async Task MigrateSessionCart(string sessionId, AppUser user)
        {
            var cartsDb = await _appDbContext.ShoppingCarts.Where(sc => sc.SessionId == sessionId).ToListAsync();
            foreach (var item in cartsDb)
            {
                item.UserId = user.Id;
            }
            await _appDbContext.SaveChangesAsync();
        }
    }
}
