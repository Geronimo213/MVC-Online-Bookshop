using Bookshop.DataAccess.Data;
using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;

namespace Bookshop.DataAccess.Repository
{
    public class CarouselRepository : Repository<Carousel>, ICarouselRepository
    {
        private AppDBContext _dbContext;
        public CarouselRepository(AppDBContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }
        public void Update(Carousel carousel)
        {
            _dbContext.Carousels.Update(carousel);
        }
    }
}
