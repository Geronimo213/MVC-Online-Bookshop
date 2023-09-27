using System.Net;
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
    public class ProductController(IUnitOfWork UnitOfWork, IWebHostEnvironment appEnvironment) : Controller
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
        CREATE OR UPDATE PRODUCT
        ************************************/
        //Get result for Create Book page
        public IActionResult Upsert(int? id)
        {
            ProductVM productVm = new ProductVM()
            {
                CategoryList = UnitOfWork.CategoryRepository.GetAll().Select(x => new SelectListItem {Text = x.Name, Value = x.Id.ToString()}),
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                return View(productVm);
            }
            else
            {
                productVm.Product = UnitOfWork.ProductRepository.Get((x => x.Id == id));
                return View(productVm);
            }
        }
        //Post method handler for Create Book, being passed a Book to work with
        [HttpPost]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (file is not null)
                {
                    string filename = obj.Product.ImageURL ?? Guid.NewGuid().ToString() + @"Images\Product\" + file.FileName;
                    string path = Path.Combine(appEnvironment.WebRootPath, filename);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);

                    }

                    using (Stream fs = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(fs);
                    }

                    obj.Product.ImageURL = filename;
                }

                if (obj.Product.Id == 0)
                {
                    UnitOfWork.ProductRepository.Add(obj.Product);
                }
                else
                {
                    UnitOfWork.ProductRepository.Update(obj.Product);
                }
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
            if (toDelete.ImageURL is not null)
            {
                string filename = toDelete.ImageURL;
                string path = Path.Combine(appEnvironment.WebRootPath, filename);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);

                }
            }
            UnitOfWork.ProductRepository.Delete(toDelete);
            UnitOfWork.Save();
            TempData["success"] = $"Book {toDelete.Title} removed successfully!";
            return RedirectToAction("Index", "Product");
        }

    }
}
