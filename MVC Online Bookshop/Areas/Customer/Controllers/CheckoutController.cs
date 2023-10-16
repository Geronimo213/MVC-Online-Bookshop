using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Bookshop.Models.ViewModels;
using Bookshop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace MVC_Online_Bookshop.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CheckoutController : Controller
    {
        private IUnitOfWork UnitOfWork { get; set; }

        public CheckoutController(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity?)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            List<ShoppingCart> items = await UnitOfWork.ShoppingCartRepository.GetAll(includeOperators: "Product").Where(x => x.UserId == userId).ToListAsync();

            if (items.Count < 1)
            {
                TempData["warning"] = "Try adding some items to your cart first!";
                return RedirectToAction("Index", "Cart");
            }

            var order = new Order()
            {
                Name = claimsIdentity?.FindFirst(ClaimTypes.GivenName)?.Value ?? "",
                PhoneNumber = claimsIdentity?.FindFirst(ClaimTypes.HomePhone)?.Value ?? "",
                ShipStreetAddress = claimsIdentity?.FindFirst(ClaimTypes.StreetAddress)?.Value ?? "",
                ShipCity = claimsIdentity?.FindFirst(ClaimTypes.Locality)?.Value ?? "",
                ShipState = claimsIdentity?.FindFirst(ClaimTypes.StateOrProvince)?.Value ?? "",
                ShipPostalCode = claimsIdentity?.FindFirst(ClaimTypes.PostalCode)?.Value ?? "",
                UserId = userId ?? ""
            };

            CheckoutVM model = new CheckoutVM()
            {
                Order = order,
                Items = items
            };

            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmOrder(CheckoutVM checkoutVm)
        {
            if (ModelState.IsValid)
            {
                List<ShoppingCart> items = await UnitOfWork.ShoppingCartRepository.GetAll(includeOperators: "Product").Where(x => x.UserId == checkoutVm.Order.UserId).ToListAsync();
                checkoutVm.Items = items;
                checkoutVm.Order.PlaceDate = DateTime.Now;
                UnitOfWork.OrderRepository.Add(checkoutVm.Order);
                UnitOfWork.Save();

                foreach (var vmItem in checkoutVm.Items)
                {
                    OrderLines orderLines = new OrderLines()
                    {
                        OrderId = checkoutVm.Order.OrderId,
                        ProductId = vmItem.ProductId,
                        Quantity = vmItem.Count
                    };
                    UnitOfWork.OrderLinesRepository.Add(orderLines);
                }
                UnitOfWork.ShoppingCartRepository.DeleteRange(items);
                UnitOfWork.Save();

                HttpContext.Session.Clear();
                return View(checkoutVm);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
