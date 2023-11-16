using Bookshop.Models;

namespace Bookshop.DataAccess.Repository.IRepository
{
    public interface ICarouselRepository : IRepository<Carousel>
    {
        void Update(Carousel carousel);
    }
}
