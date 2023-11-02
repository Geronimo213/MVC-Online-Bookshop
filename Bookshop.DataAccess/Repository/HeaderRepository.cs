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
    public class HeaderRepository : Repository<Header>, IHeaderRepository
    {
        private readonly AppDBContext _dbContext;
        public HeaderRepository(AppDBContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }
        public void Update(Header header)
        {
            _dbContext.Headers.Update(header);
        }
    }
}
