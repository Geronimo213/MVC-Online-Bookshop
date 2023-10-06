using System.Drawing;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Bookshop.DataAccess.Data;
using Bookshop.Models;
using Bookshop.DataAccess.Repository;
using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models.ViewModels;
using Bookshop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MVC_Online_Bookshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.RoleAdmin)]
    public class ProductController : Controller
    {
        private IUnitOfWork UnitOfWork { get; }
        private IWebHostEnvironment appEnvironment { get; set; }

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment appEnvironment)
        {
            this.UnitOfWork = unitOfWork;
            this.appEnvironment = appEnvironment;
        }


        /************************************
        RECIEVE PRODUCTS (INDEX)
        ************************************/

        //Handler for main Category page
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber)
        {
            ViewData["TitleSortParam"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["AuthorSortParam"] = sortOrder == "Author" ? "auth_desc" : "Author";
            ViewData["CategorySortParam"] = sortOrder == "Category" ? "cat_desc" : "Category";
            ViewData["PriceSortParam"] = sortOrder == "Price" ? "price_desc" : "Price";

            if (!String.IsNullOrEmpty(searchString))
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            var books =  UnitOfWork.ProductRepository.GetAll(includeOperators: "Category");
            books = String.IsNullOrEmpty(searchString) ? books : books.Where(s => 
                s.Title.Contains(searchString)
                || s.Author!.Contains(searchString)
                || s.ISBN!.Contains(searchString)
                || s.Category!.Name.Contains(searchString));

            switch (sortOrder)
            {
                case "title_desc":
                    books = books.OrderByDescending(s => s.Title);
                    break;
                case "Author":
                    books = books.OrderBy(s => s.Author);
                    break;
                case "auth_desc":
                    books = books.OrderByDescending(s => s.Author);
                    break;
                case "Category":
                    books = books.OrderBy(s => s.Category!.Name);
                    break;
                case "cat_desc":
                    books = books.OrderByDescending(s => s.Category!.Name);
                    break;
                case "Price":
                    books = books.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    books = books.OrderByDescending(s => s.Price);
                    break;
                default:
                    books = books.OrderBy(s => s.Title);
                    break;
            }


            int pageSize = SD.PageSizeProduct;
            return View(await PaginatedList<Product>.CreateAsync(books.AsNoTracking(), pageNumber ?? 1, pageSize));
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
                    string filename = obj.Product.ImageURL ?? @"Images\Product\" + Guid.NewGuid().ToString() + file.FileName;
                    string path = Path.Combine(appEnvironment.WebRootPath, filename);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                        filename = @"Images\Product\" + Guid.NewGuid().ToString() + file.FileName;
                        path = Path.Combine(appEnvironment.WebRootPath, filename);

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
            Product productFromDb = UnitOfWork.ProductRepository.Get(c => c.Id == id, includeOperators: "Category");

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
