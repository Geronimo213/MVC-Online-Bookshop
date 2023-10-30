using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;
using Bookshop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using Bookshop.Models.ViewModels;
using LinqKit;

namespace MVC_Online_Bookshop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IUnitOfWork UnitOfWork { get; }

        public HomeController(IUnitOfWork unitOfWork, ILogger<HomeController> logger)
        {
            this.UnitOfWork = unitOfWork;
            this._logger = logger;
        }


        public async Task<IActionResult> Index()
        {
            var books = await UnitOfWork.ProductRepository.GetAll().ToListAsync();
            return View(books);
        }

        public async Task<IActionResult> BookDetails(int? productId)
        {
            if (productId == null)
            {
                NotFound();

            }
            ShoppingCart cart = new()
            {
                Product = await UnitOfWork.ProductRepository.Get(x => x.Id == productId, includeOperators: "Categories") ?? new Product(),
                ProductId = (int)productId!,
                Count = 1
            };
            //var book = unitOfWork.ProductRepository.Get(x => x.Id == id, includeOperators: "Category");

            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> BookDetails(ShoppingCart cart)
        {
            var claimsIdentity = (ClaimsIdentity?)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            cart.UserId = userId ?? "";
            var book = await UnitOfWork.ProductRepository.Get(x => x.Id == cart.ProductId) ?? new Product() { Title = "BOOK_NOT_FOUND" };

            ShoppingCart? cartFromDb = await UnitOfWork.ShoppingCartRepository.Get(x => x.UserId == userId && x.ProductId == cart.ProductId);

            if (cartFromDb != null)
            {
                cartFromDb.Count += cart.Count;
                UnitOfWork.ShoppingCartRepository.Update(cartFromDb);
                await UnitOfWork.SaveAsync();
            }
            else
            {
                UnitOfWork.ShoppingCartRepository.Add(cart);
                await UnitOfWork.SaveAsync();

                HttpContext.Session.SetInt32(SD.SessionCart,
                    await UnitOfWork.ShoppingCartRepository.GetAll().CountAsync(x => x.UserId == userId));
            }

            TempData["success"] = $"Added {book.Title} to the cart!";

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Shop(string? id, int? pageIndex, int? pageSize, string? searchParam, string? sortOrder)
        {
            var category = await UnitOfWork.CategoryRepository.Get(x => x.Name == id, includeOperators:"Products") ?? new Category();
            var productQuery = UnitOfWork.ProductRepository.GetAll(x => x.Categories.Contains(category));
            if (productQuery is null)
            {
                return View();

            }
            if (!string.IsNullOrEmpty(searchParam))
            {
                var productSearchPredicate = PredicateBuilder.New<Product>();
                productSearchPredicate = productSearchPredicate.Or(x => x.Title.Contains(searchParam))
                    .Or(x => x.Author!.Contains(searchParam))
                    .Or(x => x.ISBN!.Contains(searchParam));
                productQuery = productQuery.Where(productSearchPredicate);
            }

            productQuery = sortOrder switch
            {
                "Title" => productQuery.OrderBy(x => x.Title),
                "Author" => productQuery.OrderBy(x => x.Author),
                _ => productQuery.OrderBy(x => x.Id)
            };
            var products = new PaginatedProductVM()
            {
                CategoryName = category.Name,
                Products = await PaginatedList<Product>.CreateAsync(productQuery, pageIndex ?? 1, pageSize ?? 25),
                SearchParam = searchParam,
                SortOrder = sortOrder

            };
            return View(products);
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
