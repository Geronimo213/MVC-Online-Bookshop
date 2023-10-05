using Bookshop.DataAccess.Repository;
using Bookshop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Bookshop.DataAccess.Data;
using Bookshop.DataAccess.Repository.IRepository;

namespace MVC_Online_Bookshop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private IUnitOfWork UnitOfWork { get; }

        public CartController(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            List<ShoppingCart> items = UnitOfWork.ShoppingCartRepository.GetAll(includeOperators: "User,Product").Where(x => x.UserId == userId).ToList();

            return View(items);
        }
    }
}
