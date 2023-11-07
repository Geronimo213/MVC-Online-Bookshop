using Bookshop.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MVC_Online_Bookshop.ViewComponents
{
    public class HeaderCarouselViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public HeaderCarouselViewComponent(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var slides = await _unitOfWork.HeaderRepository.GetAll().OrderBy(slide => slide.DisplayOrder).ToListAsync();

            return View(slides);
        }
    }
}
