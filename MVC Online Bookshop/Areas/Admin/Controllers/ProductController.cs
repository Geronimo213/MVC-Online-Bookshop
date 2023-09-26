using Microsoft.AspNetCore.Mvc;
using Bookshop.DataAccess.Data;
using Bookshop.Models;
using Bookshop.DataAccess.Repository;
using Bookshop.DataAccess.Repository.IRepository;

namespace MVC_Online_Bookshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController(IUnitOfWork unitOfWork) : Controller
    {
        private IUnitOfWork UnitOfWork { get; } = unitOfWork;


        /************************************
        RECIEVE PRODUCTS (INDEX)
        ************************************/

        //Handler for main Category page
        public IActionResult Index()
        {
            List<Product> objProductList = UnitOfWork.ProductRepository.GetAll().ToList();
            return View(objProductList);
        }

        /************************************
        CREATE PRODUCT
        ************************************/
        //Get result for Create Book page
        public IActionResult Create()
        {
            return View();
        }
        //Post method handler for Create Book, being passed a Book to work with
        [HttpPost]
        public IActionResult Create(Product obj)
        {
            if (ModelState.IsValid)
            {
                UnitOfWork.ProductRepository.Add(obj);
                UnitOfWork.Save();
                //Save to tempdata for accessing on next view w/ toastr.
                TempData["success"] = $"Book {obj.Title} created successfully!";
                return RedirectToAction("Index", "Product");
            }
            return View();
        }



        /************************************
        UPDATE PRODUCT
        ************************************/
        //Get for Edit Category page. Takes in an id passed to it from form via POST.
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product productFromDb = UnitOfWork.ProductRepository.Get(c => c.Id == id);
            return View(productFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Product obj)
        {
            UnitOfWork.ProductRepository.Update(obj); //Update the model and save changes.
            UnitOfWork.Save();
            TempData["success"] = $"Book {obj.Title} updated successfully!";
            return RedirectToAction("Index", "Product");
        }



        /************************************
        DELETE PRODUCT
        ************************************/
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) //Null check for id
            {
                return NotFound();
            }
            Product productFromDb = UnitOfWork.ProductRepository.Get(c => c.Id == id);

            return View(productFromDb);
        }

        [HttpPost]
        public IActionResult Delete(Product toDelete)
        {

            UnitOfWork.ProductRepository.Delete(toDelete);
            UnitOfWork.Save();
            TempData["success"] = $"Book {toDelete.Title} removed successfully!";
            return RedirectToAction("Index", "Product");
        }

    }
}
