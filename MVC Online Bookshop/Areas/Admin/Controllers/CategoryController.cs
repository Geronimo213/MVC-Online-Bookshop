using Microsoft.AspNetCore.Mvc;
using Bookshop.DataAccess.Data;
using Bookshop.Models;
using Bookshop.DataAccess.Repository;
using Bookshop.DataAccess.Repository.IRepository;

namespace MVC_Online_Bookshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private IUnitOfWork UnitOfWork { get; }

        public CategoryController(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }


        /************************************
        RECIEVE CATEGORIES (INDEX)
        ************************************/

        //Handler for main Category page
        public IActionResult Index()
        {
            List<Category> objCategoryList = UnitOfWork.CategoryRepository.GetAll().ToList();
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
                UnitOfWork.CategoryRepository.Add(obj);
                UnitOfWork.Save();
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
            if (id == null || id == 0)
            {
                return NotFound();
            }
            /*Category? categoryFromDb = _dbContext.Categories.Find(id);*/ //Nullable category, which is checked on next line.
            Category? categoryFromDb = UnitOfWork.CategoryRepository.Get(c => c.Id == id);
            //if (categoryFromDb == null) { return NotFound(); } //If find returned null, NotFound
            return View(categoryFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            UnitOfWork.CategoryRepository.Update(obj); //Update the model and save changes.
            UnitOfWork.Save();
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
            Category? categoryFromDb = UnitOfWork.CategoryRepository.Get(c => c.Id == id);
            if (categoryFromDb == null) { return NotFound(); }

            return View(categoryFromDb);
        }

        [HttpPost]
        public IActionResult Delete(Category toDelete)
        {
            //Category? toDelete = _dbContext.Categories.Find(id); //Old code from messing with retrieving specific fields from POSTed model as parameter.
            //if (toDelete == null) { return NotFound(); }

            UnitOfWork.CategoryRepository.Delete(toDelete);
            UnitOfWork.Save();
            TempData["success"] = $"Category {toDelete.Name} removed successfully!";
            return RedirectToAction("Index", "Category");
        }

    }
}
