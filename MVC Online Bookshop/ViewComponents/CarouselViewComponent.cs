﻿using Bookshop.DataAccess.Repository.IRepository;
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
            var categoryDb = await UnitOfWork.CategoryRepository.Get(x => x.Id == carousel.CategoryId, includeOperators:"Products", tracked:false);
            if (categoryDb is null) return View();

            var sortOrder = carousel.SortOrder;

            var productsDb = categoryDb.Products.ToList();

            productsDb = sortOrder switch
            {
                "Author" => [.. productsDb.OrderBy(x => x.Author)],
                "Title" => [.. productsDb.OrderBy(x => x.Title)],
                _ => [.. productsDb.OrderBy(x => x.Id)]
            };


            var vm = new CarouselVM()
            {
                Products = productsDb.Take(SD.MaxBooksPerSlider).ToList(),
                Title = carousel.Title
            };
            return View(vm);
        }
    }

    public class CarouselVM
    {
        public string Title { get; set; } = string.Empty;
        public List<Product> Products { get; set; } = [];
    }
}
