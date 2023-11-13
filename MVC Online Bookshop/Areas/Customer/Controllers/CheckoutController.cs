using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;
using Bookshop.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Bookshop.Utility;
using Microsoft.AspNetCore.Identity;
using Stripe.Checkout;

namespace MVC_Online_Bookshop.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CheckoutController : Controller
    {
        private IUnitOfWork UnitOfWork { get; set; }
        private readonly UserManager<AppUser> _userManager;
        public CheckoutController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            this.UnitOfWork = unitOfWork;
            this._userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity?)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return NotFound();

            var user = await _userManager.FindByIdAsync(userId);
            List<ShoppingCart> items = await UnitOfWork.ShoppingCartRepository.GetAll(includeOperators: "Product").Where(x => x.UserId == userId).ToListAsync();

            if (items.Count < 1)
            {
                TempData["warning"] = "Try adding some items to your cart first!";
                return RedirectToAction("Index", "Cart");
            }

            var order = new Order()
            {
                Name = user?.Name ?? "",
                PhoneNumber = user?.PhoneNumber ?? "",
                ShipStreetAddress = user?.StreetAddress ?? "",
                ShipCity = user?.City ?? "",
                ShipState = user?.State ?? "",
                ShipPostalCode = user?.PostalCode ?? "",
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
        public async Task<IActionResult> Index(CheckoutVM checkoutVm)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));

            var items = await UnitOfWork.ShoppingCartRepository.GetAll(includeOperators: "Product").Where(x => x.UserId == checkoutVm.Order.UserId).ToListAsync();
            checkoutVm.Items = items;
            checkoutVm.Order.PlaceDate = DateTime.Now;
            checkoutVm.Order.OrderStatus = SD.OrderPlaced;
            UnitOfWork.OrderRepository.Add(checkoutVm.Order);
            await UnitOfWork.SaveAsync();

            foreach (var vmItem in checkoutVm.Items)
            {
                var orderLines = new OrderLines()
                {
                    OrderId = checkoutVm.Order.OrderId,
                    ProductId = vmItem.ProductId,
                    Quantity = vmItem.Count
                };
                UnitOfWork.OrderLinesRepository.Add(orderLines);
                await UnitOfWork.SaveAsync();
            }
            
            //Setup stripe options
            const string domain = "https://localhost:7212/";

            var options = new SessionCreateOptions
            {
                SuccessUrl = Url.Action(action:"ConfirmOrder", controller: "Checkout", values: new { id = checkoutVm.Order.OrderId}, protocol:"https"),
                CancelUrl = Url.Action(action:"CancelOrder", controller:"Checkout", values: new {id = checkoutVm.Order.OrderId}, protocol:"https"),
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };
            //Create stripe items from checkout lines
            foreach (var item in checkoutVm.Items)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price! * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title,
                            Description = $"by {item.Product.Author}",
                            //Can implement once I'm not hosting images on local
                            //Images = new List<string> { domain + item.Product.ImageURL}
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionLineItem);
            }
            //Create stripe service
            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            if (!string.IsNullOrEmpty(session.Id))
            {
                UnitOfWork.OrderRepository.UpdateStripe(checkoutVm.Order.OrderId, session.Id, session.PaymentIntentId);
                UnitOfWork.OrderRepository.UpdateStatus(checkoutVm.Order.OrderId, SD.PaymentPending);
                await UnitOfWork.SaveAsync();
            }
            //Redirect to stripe checkout page
            Response.Headers.Append("Location", session.Url);
            return new StatusCodeResult(303);


        }

        /// <summary>
        /// Endpoint for returning from Stripe payment successfully.
        /// </summary>
        /// <param name="id">Order Id</param>
        /// <returns>ConfirmOrder Index View</returns>
        public async Task<IActionResult> ConfirmOrder(int id)
        {

            var claimsIdentity = (ClaimsIdentity?)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var orderDb = await UnitOfWork.OrderRepository.Get(x => x.OrderId == id, includeOperators:"User");
            if (orderDb == null || orderDb.UserId != userId) return NotFound();
            if (string.IsNullOrEmpty(orderDb.PaymentIntentId) && !string.IsNullOrEmpty(orderDb.SessionId))
            {
                var service = new SessionService();
                var session = await service.GetAsync(orderDb.SessionId);

                if (session.PaymentStatus.Equals("paid", StringComparison.CurrentCultureIgnoreCase))
                {
                    orderDb.PaymentIntentId = session.PaymentIntentId;
                    orderDb.OrderStatus = SD.PaymentProcessed;
                    await UnitOfWork.SaveAsync();
                }

                var cartDb = await UnitOfWork.ShoppingCartRepository.GetAll(x => x.UserId == orderDb.UserId)!.ToListAsync();
                UnitOfWork.ShoppingCartRepository.DeleteRange(cartDb);
                await UnitOfWork.SaveAsync();
                HttpContext.Session.Clear();
            }
            

            var orderVm = new OrderVM()
            {
                Header = orderDb,
                Lines = await UnitOfWork.OrderLinesRepository.GetAll(x => x.OrderId == orderDb.OrderId)!.Include(x => x.Product).ToListAsync()
            };
            return View(orderVm);
        }
        /// <summary>
        /// Endpoint for handling user cancellation of Stripe checkout. Expires Stripe session and deletes order from DB. Then redirects to cart.
        /// </summary>
        /// <param name="id">Order Id</param>
        /// <returns>RedirectToAction Action: Index, Controller: Cart</returns>
        public async Task<IActionResult> CancelOrder(int id)
        {
            var orderDb = await UnitOfWork.OrderRepository.Get(x => x.OrderId == id);
            if (orderDb == null) return NotFound();

            var service = new SessionService();

            await service.ExpireAsync(orderDb.SessionId);

            UnitOfWork.OrderRepository.Delete(orderDb);
            await UnitOfWork.SaveAsync();

            return RedirectToAction("Index", "Cart");
        }
    }
}
