using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;
using Bookshop.Models.ViewModels;
using Bookshop.Utility;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System.Security.Claims;

namespace MVC_Online_Bookshop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CheckoutController : Controller
    {
        private IUnitOfWork UnitOfWork { get; set; }
        private readonly UserManager<AppUser> _userManager;
        private readonly IAppEmailSender _emailSender;
        private IHttpContextAccessor _contextAccessor;
        public CheckoutController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IAppEmailSender emailSender, IHttpContextAccessor contextAccessor)
        {
            this.UnitOfWork = unitOfWork;
            this._userManager = userManager;
            this._emailSender = emailSender;
            this._contextAccessor = contextAccessor;
        }
        /// <summary>
        /// Checkout Index used to confirm order details and collect user shipping and billing information.
        /// </summary>
        /// <returns>Checkout Index View with CheckoutVM viewmodel.</returns>
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            AppUser? user;

            var cartCookie = CartHelper.GetCartCookie(this.HttpContext);

            if (!string.IsNullOrEmpty(userId))
            {
                user = await _userManager.FindByIdAsync(userId);
            }
            else
            {
                user = new AppUser() { Name = string.Empty };
            }

            if (user is null) return NotFound();

            var items = await UnitOfWork.ShoppingCartRepository.GetAll().Where(sc => sc.UserId == userId || sc.SessionId == cartCookie)
                .Include(sc => sc.Product).AsNoTracking().ToListAsync();
            if (items.Count < 1)
            {
                TempData["warning"] = "Try adding some items to your cart first!";
                return RedirectToAction("Index", "Cart");
            }

            var order = new Order()
            {
                Name = user.Name,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                ShipStreetAddress = user.StreetAddress ?? string.Empty,
                ShipCity = user.City ?? string.Empty,
                ShipState = user.State ?? string.Empty,
                ShipPostalCode = user.PostalCode ?? string.Empty,
                UserId = userId,
                Email = user.Email ?? string.Empty
            };

            var model = new CheckoutVM
            {
                Order = order,
                Items = items,
                SessionId = cartCookie
            };


            return View(model);
        }

        /// <summary>
        /// Accepts post from checkout View, taking all shipping and billing details from the user and
        /// confirming model validity. Then creates order header and uses passed shopping cart line ids to get information.
        /// Then creates Stripe session with order details and redirects user.
        /// </summary>
        /// <param name="checkoutVm">Model from the checkout view</param>
        /// <returns>Redirect to Stripe payment or redirect to "Checkout Index" on error.</returns>
        [HttpPost]
        public async Task<IActionResult> Index(CheckoutVM checkoutVm)
        {
            if (!ModelState.IsValid || checkoutVm.ItemIds is null) return RedirectToAction(nameof(Index));

            var claimsIdentity = (ClaimsIdentity?)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var items = await GetCheckoutItems(checkoutVm, userId ?? string.Empty);
            if (items is null) return RedirectToAction(nameof(Index));

            await SetOrderLines(items, checkoutVm);

            //Set strip options
            var stripeOptions = await Task.Run(() => SetStripeOptions(checkoutVm));
            //Create stripe service
            var session = await Task.Run(() => UpdateStripeSession(checkoutVm, stripeOptions));
            //Redirect to stripe checkout page
            Response.Headers.Append("Location", session.Url);
            return new StatusCodeResult(303);
        }

        private async Task SetOrderLines(List<ShoppingCart> items, CheckoutVM checkoutVm)
        {
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
        }
        private async Task<List<ShoppingCart>?> GetCheckoutItems(CheckoutVM checkoutVm, string userId)
        {
            var itemPredicate = PredicateBuilder.New<ShoppingCart>();
            foreach (var itemId in checkoutVm.ItemIds!)
            {
                var currentId = itemId;
                itemPredicate.Or(item => item.Id == currentId);
            }

            var itemsQuery = UnitOfWork.ShoppingCartRepository.GetAll(x => x.UserId == userId || x.SessionId == checkoutVm.SessionId);
            if (itemsQuery is null)
            {
                TempData["error"] = "User has no items in cart";
                return null;
            }

            var items = await itemsQuery.Where(itemPredicate).Include(sc => sc.Product).AsNoTracking().ToListAsync();
            if (items.Count < 1)
            {
                TempData["error"] = "Checkout item line ids user mismatch.";
                return null;
            }

            return items;
        }
        private async Task<Session> UpdateStripeSession(CheckoutVM checkoutVm, SessionCreateOptions stripeOptions)
        {
            var service = new SessionService();
            Session session = await service.CreateAsync(stripeOptions);

            if (string.IsNullOrEmpty(session.Id)) return session;
            UnitOfWork.OrderRepository.UpdateStripe(checkoutVm.Order.OrderId, session.Id, session.PaymentIntentId);
            UnitOfWork.OrderRepository.UpdateStatus(checkoutVm.Order.OrderId, SD.PaymentPending);
            await UnitOfWork.SaveAsync();

            return session;
        }
        private SessionCreateOptions SetStripeOptions(CheckoutVM checkoutVm)
        {
            //Setup stripe options
            var options = new SessionCreateOptions
            {
                SuccessUrl = Url.Action(action: "ConfirmOrder", controller: "Checkout", values: new { id = checkoutVm.Order.OrderId }, protocol: "https"),
                CancelUrl = Url.Action(action: "CancelOrder", controller: "Checkout", values: new { id = checkoutVm.Order.OrderId }, protocol: "https"),
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

            return options;
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
            var cartCookie = CartHelper.GetCartCookie(this.HttpContext);

            var orderDb = await UnitOfWork.OrderRepository.Get(x => x.OrderId == id || x.SessionId == cartCookie);
            if (orderDb == null || orderDb.UserId != userId) return NotFound();

            if (string.IsNullOrEmpty(orderDb.PaymentIntentId) && !string.IsNullOrEmpty(orderDb.SessionId))
            {
                await SetPayment(orderDb);

                if (orderDb.User is null && string.IsNullOrEmpty(orderDb.Email))
                {
                    TempData["error"] = "Email missing.";
                }
                else
                {
                    await SendEmailAsync(orderDb);
                }

            }
            var orderVm = new OrderVM()
            {
                Header = orderDb,
                Lines = await UnitOfWork.OrderLinesRepository.GetAll(x => x.OrderId == orderDb.OrderId)!.Include(x => x.Product).ToListAsync()
            };
            return View(orderVm);
        }

        private async Task SetPayment(Order orderDb)
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

        private async Task SendEmailAsync(Order orderDb)
        {
            var orderLinesDb = await UnitOfWork.OrderLinesRepository.GetAll()
                .Where(ol => ol.OrderId == orderDb.OrderId)
                .Include(ol => ol.Product)
                .AsNoTracking().ToListAsync();
            var orderLinesData = new object[orderLinesDb.Count];
            var domainRoot = await Request.GetBaseUrl();

            for (int i = 0; i < orderLinesDb.Count; i++)
            {
                orderLinesData[i] = new
                {
                    ImageSource = domainRoot + orderLinesDb[i].Product.ImageURL,
                    Title = orderLinesDb[i].Product.Title,
                    Quantity = orderLinesDb[i].Quantity,
                    Total = (orderLinesDb[i].Quantity * orderLinesDb[i].Product.Price)?.ToString("C")
                };
            }

            var templateData = new
            {
                RecipientName = orderDb.Name,
                OrderNumber = orderDb.OrderId,
                OrderTotal = orderLinesDb.Sum(ol => ol.Quantity * ol.Product.Price)?.ToString("C"),
                ShipStreet = orderDb.ShipStreetAddress,
                ShipCity = orderDb.ShipCity,
                ShipState = orderDb.ShipState,
                ShipZip = orderDb.ShipPostalCode,
                OrderLines = orderLinesData

            };
            await _emailSender.SendEmailTemplateAsync(orderDb.Email!, SD.ConfirmOrderTemplate, templateData);
        }

        /// <summary>
        /// Endpoint for handling user cancellation of Stripe checkout. Expires Stripe session and deletes order from DB. Then redirects to cart.
        /// </summary>
        /// <param name="id">Order Id</param>
        /// <returns>RedirectToAction Action: Index, Controller: Cart or NotFound on failed order lookup</returns>
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
