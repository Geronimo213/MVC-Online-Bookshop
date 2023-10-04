using Microsoft.AspNetCore.Mvc;
using Bookshop.DataAccess.Data;
using Bookshop.Models;
using Bookshop.DataAccess.Repository;
using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Utility;
using Microsoft.AspNetCore.Authorization;

namespace MVC_Online_Bookshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
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
        CREATE OR UPDATE CATEGORY
        ************************************/
        //Get result for Create Category page

        public IActionResult Upsert(int? id)
        {
            var category = new Category();
            if (id == null || id == 0)
            {
                return View(category);
            }
            else
            {
                category = UnitOfWork.CategoryRepository.Get(x => x.Id == id);
                return View(category);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Category obj)
        {
            if (ModelState.IsValid)
            {
                bool create = obj.Id == 0;
                if (create)
                {
                    UnitOfWork.CategoryRepository.Add(obj);
                }
                else
                {
                    UnitOfWork.CategoryRepository.Update(obj);
                }
                UnitOfWork.Save();
                TempData["success"] = $"Category {obj.Name} {(create ? "created" : "updated")} successfully!";
                return RedirectToAction("Index", "Category");
            }
            else
            {
                return View(obj);
            }
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
