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

        public async Task<IViewComponentResult> InvokeAsync(string? currentCategory, bool? displayBadges, string? search)
        {
            ViewData["DisplayBadges"] = displayBadges ?? true;
            ViewData["CategoryParam"] = currentCategory ?? "";
            ViewData["SearchParam"] = search;
            var categories = await UnitOfWork.CategoryRepository
                .GetAll()
                .AsNoTracking()
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new Category() 
                    { DisplayOrder = 
                        x.DisplayOrder,
                        Id = x.Id,
                        Name = x.Name,
                        ProductCount = string.IsNullOrEmpty(search) ? 
                            x.Products.Count : 
                            x.Products.Count(p => p.Title.Contains(search) || p.Author!.Contains(search) || p.ISBN!.Contains(search))
                    })
                .ToListAsync();

            return View(categories);
        }
    }
}
