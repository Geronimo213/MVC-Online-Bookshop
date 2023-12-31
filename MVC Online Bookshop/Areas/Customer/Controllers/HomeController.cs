using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;
using Bookshop.Models.ViewModels;
using Bookshop.Utility;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace MVC_Online_Bookshop.Areas.Customer.Controllers
{
    [Area(SD.RoleCustomer)]
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
            var vm = new FrontPageVM
            {
                Carousels = await UnitOfWork.CarouselRepository.GetAll().AsNoTracking().ToListAsync()
            };

            return View(vm);
        }

        public async Task<IActionResult> BookDetails(int? productId)
        {
            if (productId is null)
            {
                NotFound();

            }
            ShoppingCart cart = new()
            {
                Product = await UnitOfWork.ProductRepository.Get(x => x.Id == productId, includeOperators: "Categories", tracked:false) ?? new Product(),
                ProductId = (int)productId!,
                Count = 1
            };

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> BookDetails(ShoppingCart cart)
        {
            var claimsIdentity = (ClaimsIdentity?)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cartCookie = CartHelper.GetCartCookie(this.HttpContext);

            cart.SessionId = cartCookie;
            cart.UserId = userId;
            var book = await UnitOfWork.ProductRepository.Get(x => x.Id == cart.ProductId, tracked:false) ?? new Product() { Title = "BOOK_NOT_FOUND" };

            await AddToCartAsync(cart);
            TempData["success"] = $"Added {book.Title} to the cart!";

            return RedirectToAction(nameof(Index));
        }

        private async Task AddToCartAsync(ShoppingCart cart)
        {
            ShoppingCart? cartFromDb = await UnitOfWork.ShoppingCartRepository.Get(x => (x.UserId == cart.UserId || x.SessionId == cart.SessionId) && x.ProductId == cart.ProductId, tracked: false);

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
                    await UnitOfWork.ShoppingCartRepository.GetAll().CountAsync(x => x.UserId == cart.UserId || x.SessionId == cart.SessionId));
            }

        }


        public async Task<IActionResult> Shop(string? id, int? pageIndex, int? pageSize, string? searchParam, string currentFilter, string? sortOrder)
        {
            var category = await UnitOfWork.CategoryRepository.Get(x => x.Name == id);
            var productQuery = category is not null
                ? UnitOfWork.ProductRepository.GetAll(x => x.Categories.Contains(category))?.AsNoTracking()
                : UnitOfWork.ProductRepository.GetAll().AsNoTracking();
            if (productQuery is null)
            {
                return View();
            }
            if (!string.IsNullOrEmpty(searchParam))
            {
                pageIndex = 1;
            }
            else
            {
                searchParam = currentFilter ?? "";
            }

            productQuery = await Task.Run(() => SearchFilter(searchParam, productQuery));
            productQuery = sortOrder switch
            {
                "Title" => productQuery.OrderBy(x => x.Title),
                "Author" => productQuery.OrderBy(x => x.Author),
                _ => productQuery.OrderBy(x => x.Id)
            };
            var products = new PaginatedProductVM()
            {
                CategoryName = category is not null ? category.Name : "All",
                Products = await PaginatedList<Product>.CreateAsync(productQuery, pageIndex ?? 1, pageSize ?? 25),
                CurrentFilter = searchParam,
                SortOrder = sortOrder
            };
            return View(products);
        }

        private static IQueryable<Product> SearchFilter(string searchParam, IQueryable<Product> productQuery)
        {
            var searchTerms = searchParam.ToLower().Split(' ', ',', '.', ';', ':').Except(SD.StopWords);
            foreach (var term in searchTerms)
            {
                var productSearchPredicate = PredicateBuilder.New<Product>();
                productSearchPredicate = productSearchPredicate
                    .Or(x => x.Title.Contains(term))
                    .Or(x => x.Categories.Any(c => c.Name.Contains(term)))
                    .Or(x => x.Author!.Contains(term))
                    .Or(x => x.ISBN!.Contains(term));
                productQuery = productQuery.Where(productSearchPredicate);
            }

            return productQuery;
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
