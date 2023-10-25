using Bookshop.DataAccess.Data;
using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;

namespace Bookshop.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private AppDBContext DbContext { get; }

        public CategoryRepository(AppDBContext db) : base(db)
        {
            this.DbContext = db;
        }


        public void Update(Category category)
        {
            DbContext.Categories.Update(category);
        }
    }
}
