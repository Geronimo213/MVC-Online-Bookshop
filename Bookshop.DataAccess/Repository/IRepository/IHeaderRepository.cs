using Bookshop.Models;

namespace Bookshop.DataAccess.Repository.IRepository
{
    public interface IHeaderRepository : IRepository<Header>
    {
        void Update(Header header);
    }
}
