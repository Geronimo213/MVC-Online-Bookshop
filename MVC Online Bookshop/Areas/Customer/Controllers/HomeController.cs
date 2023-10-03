using Microsoft.AspNetCore.Mvc;
using Bookshop.Models;
using System.Diagnostics;
using System.Security.Claims;
using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.DataAccess.Repository;
using Microsoft.AspNetCore.Authorization;

namespace MVC_Online_Bookshop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IUnitOfWork unitOfWork { get; set; }

        public HomeController(IUnitOfWork unitOfWork, ILogger<HomeController> logger)
        {
            this.unitOfWork = unitOfWork;
            this._logger = logger;
        }


        public IActionResult Index()
        {
            var books = unitOfWork.ProductRepository.GetAll(includeOperators: "Category").ToList();
            return View(books);
        }

        public IActionResult BookDetails(int productId)
        {
            if (productId == null)
            {
                NotFound();

            }
            ShoppingCart cart = new()
            {
                Product = unitOfWork.ProductRepository.Get(x => x.Id == productId, includeOperators: "Category"),
                ProductId = productId,
                Count = 1
            };
            //var book = unitOfWork.ProductRepository.Get(x => x.Id == id, includeOperators: "Category");

            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult BookDetails(ShoppingCart cart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            cart.UserId = userId;

            ShoppingCart? cartFromDb = unitOfWork.ShoppingCartRepository.Get(x => x.UserId == userId && x.ProductId == cart.ProductId);

            if (cartFromDb != null)
            {
                cartFromDb.Count += cart.Count;
                unitOfWork.ShoppingCartRepository.Update(cartFromDb);
            }
            else
            {
                unitOfWork.ShoppingCartRepository.Add(cart);
            }

            unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
