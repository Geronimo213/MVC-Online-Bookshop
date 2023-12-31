﻿using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;
using Bookshop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MVC_Online_Bookshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.RoleAdmin)]
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
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageSize, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["DisplayOrderParam"] = string.IsNullOrEmpty(sortOrder) ? "display_order_desc" : "";
            ViewData["NameSortParam"] = sortOrder == "Name" ? "name_desc" : "Name";

            if (!string.IsNullOrEmpty(searchString))
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            pageSize ??= SD.PageSizeProduct;
            ViewData["CurrentPageSize"] = (int)pageSize;

            var categories = UnitOfWork.CategoryRepository.GetAll();
            categories = string.IsNullOrEmpty(searchString) ? categories : UnitOfWork.CategoryRepository.GetAll().Where(x =>
                x.Name.Contains(searchString)
                || x.Id.ToString().Contains(searchString));
            categories = sortOrder switch
            {
                "display_order_desc" => categories.OrderByDescending(s => s.DisplayOrder),
                "Name" => categories.OrderBy(s => s.Name),
                "name_desc" => categories.OrderByDescending(s => s.Name),
                _ => categories.OrderBy(s => s.DisplayOrder),
            };
            var categoryList = await PaginatedList<Category>.CreateAsync(categories, pageNumber ?? 1, (int)pageSize);
            return View(categoryList);
        }

        /************************************
        CREATE OR UPDATE CATEGORY
        ************************************/
        //Get result for Create Category page

        public async Task<IActionResult> Upsert(int? id)
        {
            var category = new Category();
            if (id is null or 0)
            {
                category.DisplayOrder = await UnitOfWork.CategoryRepository.GetAll().CountAsync() + 1;
                return View(category);
            }
            else
            {
                category = await UnitOfWork.CategoryRepository.Get(x => x.Id == id);
                return View(category);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(Category obj)
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
                await UnitOfWork.SaveAsync();
                TempData["success"] = $"Category {obj.Name} {(create ? "created" : "updated")} successfully!";
                return RedirectToAction("Index", "Category");
            }
            else
            {
                TempData["error"] = "Model state is invalid.";
                return View(obj);
            }
        }




        /************************************
        DELETE CATEGORY
        ************************************/
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null or 0) //Null check for id
            {
                return NotFound();
            }
            /*Category? categoryFromDb = _dbContext.Categories.Find(id);*/ //If find returns null, NotFound
            Category? categoryFromDb = await UnitOfWork.CategoryRepository.Get(c => c.Id == id);
            if (categoryFromDb == null) { return NotFound(); }

            return View(categoryFromDb);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Category toDelete)
        {
            //Category? toDelete = _dbContext.Categories.Find(id); //Old code from messing with retrieving specific fields from POSTed model as parameter.
            //if (toDelete == null) { return NotFound(); }

            UnitOfWork.CategoryRepository.Delete(toDelete);
            await UnitOfWork.SaveAsync();
            TempData["success"] = $"Category {toDelete.Name} removed successfully!";
            return RedirectToAction("Index", "Category");
        }

    }
}
