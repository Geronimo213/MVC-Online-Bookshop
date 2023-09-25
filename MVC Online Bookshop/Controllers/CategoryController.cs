using Microsoft.AspNetCore.Mvc;
using Bookshop.DataAccess.Data;
using Bookshop.Models;
using Bookshop.DataAccess.Repository;
using Bookshop.DataAccess.Repository.IRepository;

namespace MVC_Online_Bookshop.Controllers
{
    public class CategoryController(ICategoryRepository categoryRepository) : Controller
    {
        private ICategoryRepository CategoryRepository { get; } = categoryRepository;


        /************************************
        RECIEVE CATEGORIES (INDEX)
        ************************************/

        //Handler for main Category page
        public IActionResult Index()
        {
            List<Category> objCategoryList = CategoryRepository.GetAll().ToList();
            return View(objCategoryList);
        }

        /************************************
        CREATE CATEGORY
        ************************************/
        //Get result for Create Category page
        public IActionResult Create()
        {
            return View();
        }
        //Post method handler for Create Category, being passed a Category to work with
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                categoryRepository.Add(obj);
                categoryRepository.Save();
                //Save to tempdata for accessing on next view w/ toastr.
                TempData["success"] = $"Category {obj.Name} created successfully!";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }



        /************************************
        UPDATE CATEGORY
        ************************************/
        //Get for Edit Category page. Takes in an id passed to it from form via POST.
        public IActionResult Edit(int? id)
        {
            if(id == null || id == 0)
            {
                return NotFound();
            }
            /*Category? categoryFromDb = _dbContext.Categories.Find(id);*/ //Nullable category, which is checked on next line.
            Category? categoryFromDb = categoryRepository.Get(c => c.Id == id);
            //if (categoryFromDb == null) { return NotFound(); } //If find returned null, NotFound
            return View(categoryFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            categoryRepository.Update(obj); //Update the model and save changes.
            categoryRepository.Save();
            TempData["success"] = $"Category {obj.Name} updated successfully!";
            return RedirectToAction("Index", "Category");
        }



        /************************************
        DELETE CATEGORY
        ************************************/
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) //Null check for id
            {
                return NotFound();
            }
            /*Category? categoryFromDb = _dbContext.Categories.Find(id);*/ //If find returns null, NotFound
            Category? categoryFromDb = CategoryRepository.Get(c => c.Id == id);
            if (categoryFromDb == null) { return NotFound(); }

            return View(categoryFromDb);
        }

        [HttpPost]
        public IActionResult Delete(Category toDelete)
        {
            //Category? toDelete = _dbContext.Categories.Find(id); //Old code from messing with retrieving specific fields from POSTed model as parameter.
            //if (toDelete == null) { return NotFound(); }

            CategoryRepository.Delete(toDelete);
            CategoryRepository.Save();
            TempData["success"] = $"Category {toDelete.Name} removed successfully!";
            return RedirectToAction("Index", "Category");
        }

    }
}
