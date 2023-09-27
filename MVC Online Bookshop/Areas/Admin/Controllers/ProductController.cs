using Microsoft.AspNetCore.Mvc;
using Bookshop.DataAccess.Data;
using Bookshop.Models;
using Bookshop.DataAccess.Repository;
using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVC_Online_Bookshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController(IUnitOfWork UnitOfWork) : Controller
    {
        //private IUnitOfWork UnitOfWork { get; } = unitOfWork;


        /************************************
        RECIEVE PRODUCTS (INDEX)
        ************************************/

        //Handler for main Category page
        public IActionResult Index()
        {
            List<Product> objProductList = UnitOfWork.ProductRepository.GetAll().ToList();
            List<Category> objCategoryList = UnitOfWork.CategoryRepository.GetAll().ToList();
            return View(objProductList);
        }

        /************************************
        CREATE PRODUCT
        ************************************/
        //Get result for Create Book page
        public IActionResult Create()
        {
            ProductVM productVm = new ProductVM()
            {
                CategoryList = UnitOfWork.CategoryRepository.GetAll().Select(x => new SelectListItem {Text = x.Name, Value = x.Id.ToString()}),
                Product = new Product()
            };
            return View(productVm);
        }
        //Post method handler for Create Book, being passed a Book to work with
        [HttpPost]
        public IActionResult Create(ProductVM obj)
        {
            if (ModelState.IsValid)
            {
                UnitOfWork.ProductRepository.Add(obj.Product);
                UnitOfWork.Save();
                //Save to tempdata for accessing on next view w/ toastr.
                TempData["success"] = $"Book {obj.Product.Title} created successfully!";
                return RedirectToAction("Index", "Product");
            }
            else
            {
                ProductVM productVm = new ProductVM()
                {
                    CategoryList = UnitOfWork.CategoryRepository.GetAll().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }),
                    Product = new Product()
                };
                return View(productVm);
            }
            
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
