using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;
using Bookshop.Models.ViewModels;
using Bookshop.Utility;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System.Security.Claims;

namespace MVC_Online_Bookshop.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
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
            var claimsIdentity = (ClaimsIdentity?)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return NotFound();

            var user = await _userManager.FindByIdAsync(userId);
            List<ShoppingCart> items = await UnitOfWork.ShoppingCartRepository.GetAll(includeOperators: "Product").Where(x => x.UserId == userId).AsNoTracking().ToListAsync();

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

        /// <summary>
        /// Accepts post from checkout View, taking all shipping and billing details from the user and
        /// confirming model validity. Then creates order header and uses passed shopping cart line ids to get information.
        /// Then creates Stripe session with order details and redirects user.
        /// </summary>
        /// <param name="checkoutVm">Model from the checkout view</param>
        /// <returns>Redirect to Stripe payment or redirect to Checkout Index on error.</returns>
        [HttpPost]
        public async Task<IActionResult> Index(CheckoutVM checkoutVm)
        {
            if (!ModelState.IsValid || checkoutVm.ItemIds is null) return RedirectToAction(nameof(Index));

            var claimsIdentity = (ClaimsIdentity?)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var itemPredicate = PredicateBuilder.New<ShoppingCart>();

            foreach (var itemId in checkoutVm.ItemIds)
            {
                var currentId = itemId;
                itemPredicate.Or(item => item.Id == currentId);
            }

            var itemsQuery = UnitOfWork.ShoppingCartRepository.GetAll(x => x.UserId == userId);

            if (itemsQuery is null)
            {
                TempData["error"] = "User has no items in cart";
                return RedirectToAction(nameof(Index));
            }

            var items = await itemsQuery.Where(itemPredicate).Include(sc => sc.Product).AsNoTracking().ToListAsync();

            if (items.Count < 1)
            {
                TempData["error"] = "Checkout cart line ids user mismatch.";
                return RedirectToAction(nameof(Index));
            }

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

            var orderDb = await UnitOfWork.OrderRepository.Get(x => x.OrderId == id, includeOperators: "User");
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



                if (orderDb.User is null || string.IsNullOrEmpty(orderDb.User.Email))
                {
                    TempData["error"] = "Email missing.";
                }
                else
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
                        RecipientName = orderDb.User.Name,
                        OrderNumber = orderDb.OrderId,
                        OrderTotal = orderLinesDb.Sum(ol => ol.Quantity * ol.Product.Price)?.ToString("C"),
                        ShipStreet = orderDb.ShipStreetAddress,
                        ShipCity = orderDb.ShipCity,
                        ShipState = orderDb.ShipState,
                        ShipZip = orderDb.ShipPostalCode,
                        OrderLines = orderLinesData

                    };
                    await _emailSender.SendEmailTemplateAsync(orderDb.User.Email, SD.ConfirmOrderTemplate, templateData);
                }

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
