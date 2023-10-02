using Microsoft.AspNetCore.Mvc;
using Bookshop.Models;
using System.Diagnostics;
using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.DataAccess.Repository;

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

        public IActionResult BookDetails(int? id)
        {
            if (id == null)
            {
                NotFound();

            }

            var book = unitOfWork.ProductRepository.Get(x => x.Id == id, includeOperators: "Category");

            return View(book);
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
