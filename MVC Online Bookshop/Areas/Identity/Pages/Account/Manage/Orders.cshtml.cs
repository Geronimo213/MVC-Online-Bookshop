using Bookshop.Models;
using Bookshop.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Utility;
using Microsoft.EntityFrameworkCore;

namespace MVC_Online_Bookshop.Areas.Identity.Pages.Account.Manage
{
    public class OrdersModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public PaginatedList<OrderVM> Orders { get; set; }
        public OrdersModel(UserManager<AppUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> OnGetAsync(int? pageNumber)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) { return NotFound("Could not find user."); }

            var pageSize = SD.PageSizeOrder;
            var orderQuery = _unitOfWork.OrderRepository.GetAll().Where(o => o.UserId == user.Id).OrderByDescending(o => o.PlaceDate).AsNoTracking();
            var orderLinesQuery = _unitOfWork.OrderLinesRepository.GetAll().Where(ol => orderQuery.Any(x => x.OrderId == ol.OrderId)).AsNoTracking();
            Orders = await
                PaginatedOrders.CreateAsync(orderQuery, orderLinesQuery, pageNumber ?? 1, (int)pageSize, includeOperators: "Product");

            return Page();
        }
    }
}
