using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Bookshop.DataAccess.Data;
using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;
using Bookshop.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Bookshop.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private AppDBContext DbContext { get; }

        public ProductRepository(AppDBContext dbContext) : base(dbContext)
        {
            this.DbContext = dbContext;
        }
        public async Task Update(ProductVM productVm)
        {
            var product = productVm.Product;
            var productFromDb = await DbContext.Products.Include(x => x.Categories).FirstOrDefaultAsync(x => x.Id == product.Id);
            if (productFromDb is null)
            {
                DbContext.Products.Update(product);
                return;
            }

            productFromDb.Categories.Clear();
            await DbContext.SaveChangesAsync();
            DbContext.Entry(productFromDb).CurrentValues.SetValues(product);
            var availableCategories = await DbContext.Categories.ToListAsync();
            foreach (var categoryId in productVm.CategoryIds)
            {
                productFromDb.Categories.Add(availableCategories.First(x => x.Id == categoryId));
            }
        }

        public void Update(Product product)
        {
            DbContext.Products.Update(product);
        }

        public async Task Add(ProductVM productVm)
        {
            var availableCategories = await DbContext.Categories.ToListAsync();
            foreach (var categoryId in productVm.CategoryIds)
            {
                productVm.Product.Categories.Add(availableCategories.First(x => x.Id == categoryId));
            }
            DbContext.Products.Add(productVm.Product);
        }
    }
}
