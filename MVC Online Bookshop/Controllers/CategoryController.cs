using Microsoft.AspNetCore.Mvc;
using MVC_Online_Bookshop.Data;
using MVC_Online_Bookshop.Models;

namespace MVC_Online_Bookshop.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDBContext _dbContext;
        public CategoryController(AppDBContext dB)
        {
            _dbContext = dB;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _dbContext.Categories.ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Categories.Add(obj);
                _dbContext.SaveChanges();
                return RedirectToAction("Index", "Category");
            }
            return View();
        }
    }
}
