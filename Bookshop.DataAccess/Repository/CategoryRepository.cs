using Bookshop.DataAccess.Data;
using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookshop.DataAccess.Repository
{
    public class CategoryRepository(AppDBContext db) : Repository<Category>(db), ICategoryRepository
    {
        private AppDBContext DbContext { get; } = db;


        public void Update(Category category)
        {
            DbContext.Categories.Update(category);
        }
    }
}
