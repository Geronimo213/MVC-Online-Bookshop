using Bookshop.Models;
using Bookshop.Models.ViewModels;

namespace Bookshop.DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product product);
        public Task Update(ProductVM productVm);

        public Task Add(ProductVM productVm);

    }
}
