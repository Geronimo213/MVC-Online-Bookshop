using Bookshop.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookshop.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        public AppDBContext DbContext { get; }
        public ICategoryRepository CategoryRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IShoppingCartRepository ShoppingCartRepository { get; }
        public IAppUserRepository AppUserRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public IOrderLinesRepository OrderLinesRepository { get; }

        Task SaveAsync();
    }
}
