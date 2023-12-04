using Bookshop.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MVC_Online_Bookshop.ViewComponents
{
    public class NavbarListsViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public NavbarListsViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var lists = await _unitOfWork.BookListRepository.GetAll().AsNoTracking().ToListAsync();
            return View(lists);
        }
    }
}
