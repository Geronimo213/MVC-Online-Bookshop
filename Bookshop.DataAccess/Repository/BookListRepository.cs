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
    public class BookListRepository : Repository<BookList>, IBookListRepository
    {
        private readonly AppDBContext _dbContext;
        public BookListRepository(AppDBContext db) : base(db)
        {
            _dbContext = db;
        }

        public void Update(BookList bookList)
        {
            _dbContext.BookLists.Update(bookList);
        }
    }
}
