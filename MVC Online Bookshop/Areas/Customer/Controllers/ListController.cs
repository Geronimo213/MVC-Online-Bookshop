using Bookshop.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace MVC_Online_Bookshop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ListController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ListController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index(int listId)
        {
            var list = await _unitOfWork.BookListRepository.Get(bl =>
                bl.Id == listId, includeOperators:"Books");
            if (list is not null)
            {
                ViewData["ListUrl"] = Url.Action("Index", "List", new { listId = listId }, Request.Scheme);
                return View(list);
            }
            TempData["error"] = "List not found.";
            return NotFound();
        }
    }
}
