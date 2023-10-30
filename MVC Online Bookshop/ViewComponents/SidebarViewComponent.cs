using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MVC_Online_Bookshop.ViewComponents
{
    public class SidebarViewComponent : ViewComponent
    {
        private IUnitOfWork UnitOfWork { get; }

        public SidebarViewComponent(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync(string? currentCategory, bool? displayBadges)
        {
            ViewData["DisplayBadges"] = displayBadges ?? true;
            ViewData["CategoryParam"] = currentCategory ?? "";
            var categories = await UnitOfWork.CategoryRepository
                .GetAll()
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new Category() 
                    { DisplayOrder = 
                        x.DisplayOrder,
                        Id = x.Id,
                        Name = x.Name,
                        ProductCount = x.Products.Count
                    })
                .ToListAsync();

            return View(categories);
        }
    }
}
