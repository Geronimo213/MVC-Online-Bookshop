using Bookshop.DataAccess.Data;

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
        public ICarouselRepository CarouselRepository { get; }

        Task SaveAsync();
    }
}
