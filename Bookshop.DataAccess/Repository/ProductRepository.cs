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
    public class ProductRepository(AppDBContext dbContext) : Repository<Product>(dbContext), IProductRepository
    {
        private AppDBContext DbContext { get; } = dbContext;
        public void Update(Product product)
        {
            DbContext.Products.Update(product);
        }
    }
}
