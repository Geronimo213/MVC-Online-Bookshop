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
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private AppDBContext DbContext { get; }

        public ProductRepository(AppDBContext dbContext) : base(dbContext)
        {
            this.DbContext = dbContext;
        }
        public void Update(Product product)
        {
            DbContext.Products.Update(product);
        }
    }
}
