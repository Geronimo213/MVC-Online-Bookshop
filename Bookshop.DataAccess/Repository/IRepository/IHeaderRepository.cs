using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bookshop.Models;

namespace Bookshop.DataAccess.Repository.IRepository
{
    public interface IHeaderRepository : IRepository<Header>
    {
        void Update(Header header);
    }
}
