using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bookshop.DataAccess.Data;
using Bookshop.DataAccess.Repository.IRepository;

namespace Bookshop.DataAccess.Repository
{
    public class UnitOfWork(AppDBContext dBContext) : IUnitOfWork
    {
        private AppDBContext DbContext { get; } = dBContext;
        public ICategoryRepository CategoryRepository { get; init; } = new CategoryRepository(dBContext);
        public IProductRepository ProductRepository { get; init; } = new ProductRepository(dBContext);

        public void Save()
        {
            DbContext.SaveChanges();
        }
    }
}
