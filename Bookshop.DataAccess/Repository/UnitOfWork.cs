using Bookshop.DataAccess.Data;
using Bookshop.DataAccess.Repository.IRepository;

namespace Bookshop.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public AppDBContext DbContext { get; }
        public ICategoryRepository CategoryRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IShoppingCartRepository ShoppingCartRepository { get; }
        public IAppUserRepository AppUserRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public IOrderLinesRepository OrderLinesRepository { get; }
        public ICarouselRepository CarouselRepository { get; }
        public UnitOfWork(AppDBContext dbContext)
        {
            this.DbContext ??= dbContext;
            this.CategoryRepository ??= new CategoryRepository(dbContext);
            this.ProductRepository ??= new ProductRepository(dbContext);
            this.ShoppingCartRepository ??= new ShoppingCartRepository(dbContext);
            this.AppUserRepository ??= new AppUserRepository(dbContext);
            this.OrderRepository ??= new OrderRepository(dbContext);
            this.OrderLinesRepository ??= new OrderLinesRepository(dbContext);
            this.CarouselRepository = new CarouselRepository(dbContext);
        }

        public async Task SaveAsync()
        {
            try
            {
                await DbContext.SaveChangesAsync();
                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                await Task.FromException(e);
            }
        }
    }
}
