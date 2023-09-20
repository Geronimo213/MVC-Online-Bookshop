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
                TempData["success"] = $"Category {obj.Name} created successfully!";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }


        public IActionResult Edit(int? id)
        {
            if(id == null || id == 0)
            {
                return NotFound();
            }
            Category categoryFromDb = _dbContext.Categories.Find(id);
            if (categoryFromDb == null) { return NotFound(); }
            return View(categoryFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            _dbContext.Categories.Update(obj);
            _dbContext.SaveChanges();
            TempData["success"] = $"Category {obj.Name} updated successfully!";
            return RedirectToAction("Index", "Category");
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category categoryFromDb = _dbContext.Categories.Find(id);
            if (categoryFromDb == null) { return NotFound(); }

            return View(categoryFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category? toDelete = _dbContext.Categories.Find(id);
            if (toDelete == null) { return NotFound(); }
            _dbContext.Remove(toDelete);
            _dbContext.SaveChanges();
            TempData["success"] = $"Category {toDelete.Name} removed successfully!";
            return RedirectToAction("Index", "Category");
        }

    }
}
