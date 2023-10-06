using Bookshop.DataAccess.Repository;
using Bookshop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Bookshop.DataAccess.Data;
using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Utility;

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
            List<ShoppingCart> items = UnitOfWork.ShoppingCartRepository.GetAll(includeOperators: "User,Product,Product.Category").Where(x => x.UserId == userId).ToList();

            return View(items);
        }

        [HttpPost]
        public IActionResult ChangeItemCount(int cartId, string changeAction)
        {
            var cartItem =
                UnitOfWork.ShoppingCartRepository.Get(x => x.Id == cartId);
            if (cartItem != null)
            {
                switch (changeAction)
                {
                    case SD.CartIncrement:
                        cartItem.Count++;
                        break;
                    case SD.CartDecrement:
                        cartItem.Count--;
                        break;
                    default:
                        break;
                }
                UnitOfWork.ShoppingCartRepository.Update(cartItem);
                UnitOfWork.Save();
                if (cartItem.Count < 1)
                {
                    return RemoveItem(cartId);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult RemoveItem(int cartId)
        {
            var cartItem =
                UnitOfWork.ShoppingCartRepository.Get(x => x.Id == cartId, tracked:true);
            if (cartItem != null)
            {
                UnitOfWork.ShoppingCartRepository.Delete(cartItem);
                UnitOfWork.Save();
            }
            if (HttpContext.Session.GetInt32(SD.SessionCart) is not null)
            {
                HttpContext.Session.SetInt32(SD.SessionCart, (HttpContext.Session.GetInt32(SD.SessionCart) ?? 1) - 1);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
