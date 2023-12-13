using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;
using Bookshop.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
        public async Task<IActionResult> Index()
        {

            var claimsIdentity = (ClaimsIdentity?)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cartCookie = CartHelper.GetCartCookie(this.HttpContext);
            List<ShoppingCart> items = await UnitOfWork.ShoppingCartRepository.GetAll(includeOperators: "Product,Product.Categories").Where(x => x.UserId == userId || x.SessionId == cartCookie).ToListAsync();

            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeItemCount(int cartId, string changeAction)
        {
            var cartItem = await
                UnitOfWork.ShoppingCartRepository.Get(x => x.Id == cartId);

            if (cartItem == null)
            {
                TempData["error"] = "ERROR: Cart Item not Found";
                return RedirectToAction(nameof(Index));
            }

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
            await UnitOfWork.SaveAsync();
            if (cartItem.Count < 1)
            {
                return await RemoveItem(cartId);
            }

            return RedirectToAction((nameof(Index)));

        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int cartId)
        {
            var cartItem = await
                UnitOfWork.ShoppingCartRepository.Get(x => x.Id == cartId, tracked: true);
            if (cartItem != null)
            {
                UnitOfWork.ShoppingCartRepository.Delete(cartItem);
                await UnitOfWork.SaveAsync();
            }
            if (HttpContext.Session.GetInt32(SD.SessionCart) is not null)
            {
                HttpContext.Session.SetInt32(SD.SessionCart, (HttpContext.Session.GetInt32(SD.SessionCart) ?? 1) - 1);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
