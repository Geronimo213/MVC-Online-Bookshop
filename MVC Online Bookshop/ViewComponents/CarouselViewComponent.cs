using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;
using Bookshop.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MVC_Online_Bookshop.ViewComponents
{
    public class CarouselViewComponent : ViewComponent
    {
        private IUnitOfWork UnitOfWork { get; }

        public CarouselViewComponent(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync(Carousel carousel)
        {
            var categoryDb = await UnitOfWork.CategoryRepository.Get(x => x.Id == carousel.CategoryId);
            if (categoryDb is null) return View();

            var productsDb = UnitOfWork.ProductRepository.GetAll(x => x.Categories.Contains(categoryDb));
            if (productsDb is null) return View();

            var sortOrder = carousel.SortOrder;

            productsDb = sortOrder switch
            {
                "Author" => productsDb.OrderBy(x => x.Author),
                "Title" => productsDb.OrderBy(x => x.Title),
                _ => productsDb.OrderBy(x => x.Id)
            };


            var vm = new CarouselVM()
            {
                Products = await productsDb.Take(SD.MaxBooksPerSlider).ToListAsync(),
                Title = carousel.Title
            };
            return View(vm);
        }
    }

    public class CarouselVM
    {
        public string Title { get; set; }
        public List<Product> Products { get; set; }

        public CarouselVM()
        {
            this.Products = new List<Product>();
            this.Title = string.Empty;
        }
    }
}
