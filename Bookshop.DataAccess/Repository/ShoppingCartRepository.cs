using Bookshop.DataAccess.Data;
using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;

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
    }
}
