using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Bookshop.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace MVC_Online_Bookshop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CheckoutController : Controller
    {
        private IUnitOfWork UnitOfWork { get; set; }

        public CheckoutController(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            List<ShoppingCart> items = UnitOfWork.ShoppingCartRepository.GetAll(includeOperators: "Product").Where(x => x.UserId == userId).ToList();

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

        public IActionResult ConfirmOrder(CheckoutVM checkoutVm)
        {
            return View();
        }
    }
}
