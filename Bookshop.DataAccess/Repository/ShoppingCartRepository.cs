using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bookshop.DataAccess.Data;
using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;

namespace Bookshop.DataAccess.Repository
{
    internal class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private AppDBContext _appDBContext;
        public ShoppingCartRepository(AppDBContext dbContext) : base(dbContext)
        {
            this._appDBContext = dbContext;
        }

        public void Update(ShoppingCart shoppingCart)
        {
            _appDBContext.ShoppingCarts.Update(shoppingCart);
        }
    }
}
