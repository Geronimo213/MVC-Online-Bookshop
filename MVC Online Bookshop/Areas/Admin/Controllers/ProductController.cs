using System.Drawing.Drawing2D;
using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;
using Bookshop.Models.ViewModels;
using Bookshop.Utility;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp.Formats.Jpeg;

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
        RECEIVE PRODUCTS (INDEX)
        ************************************/

        //Handler for main Category page
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageSize, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["TitleSortParam"] = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["AuthorSortParam"] = sortOrder == "Author" ? "auth_desc" : "Author";
            ViewData["CategorySortParam"] = sortOrder == "Category" ? "cat_desc" : "Category";
            ViewData["PriceSortParam"] = sortOrder == "Price" ? "price_desc" : "Price";

            if (!string.IsNullOrEmpty(searchString))
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter ?? string.Empty;
            }
            ViewData["CurrentFilter"] = searchString;

            pageSize ??= SD.PageSizeProduct;
            ViewData["CurrentPageSize"] = (int)pageSize;

            var books = UnitOfWork.ProductRepository.GetAll(includeOperators: "Categories").AsNoTracking();
            var searchTerms = searchString.ToLower().Split(' ', ',', '.', ';', ':').Except(SD.StopWords);
            foreach (var term in searchTerms)
            {
                var productSearchPredicate = PredicateBuilder.New<Product>();
                productSearchPredicate = productSearchPredicate
                    .Or(x => x.Title.ToString().Contains(term))
                    .Or(x => x.Author!.Contains(term))
                    .Or(x => x.ISBN!.Contains(term))
                    .Or(x => x.Categories.Any(category => category.Name.Contains(term)));
                books = books.Where(productSearchPredicate);
            }


            books = sortOrder switch
            {
                "title_desc" => books.OrderByDescending(s => s.Title),
                "Author" => books.OrderBy(s => s.Author),
                "auth_desc" => books.OrderByDescending(s => s.Author),
                "Category" => books.OrderBy(s => s.Categories.OrderBy(x => x.DisplayOrder).FirstOrDefault()),
                "cat_desc" => books.OrderByDescending(s => s.Categories.OrderByDescending(x => x.DisplayOrder).FirstOrDefault()),
                "Price" => books.OrderBy(s => s.Price),
                "price_desc" => books.OrderByDescending(s => s.Price),
                _ => books.OrderBy(s => s.Title),
            };


            //int pageSize = SD.PageSizeProduct;
            return View(await PaginatedList<Product>.CreateAsync(books.AsNoTracking(), pageNumber ?? 1, (int)pageSize));
        }

        /************************************
        CREATE OR UPDATE PRODUCT
        ************************************/
        //Get result for Create Book page
        public async Task<IActionResult> Upsert(int? id)
        {
            var productVm = new ProductVM()
            {
                CategoryList = await UnitOfWork.CategoryRepository.GetAll().OrderBy(cat => cat.DisplayOrder).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToListAsync(),
                Product = new Product()
            };
            if (id is null or 0)
            {
                return View(productVm);
            }
            else
            {
                productVm.Product = await UnitOfWork.ProductRepository.Get((x => x.Id == id), includeOperators: "Categories") ?? new Product();
                foreach (var category in productVm.Product.Categories)
                {
                    productVm.CategoryIds.Add(category.Id);
                }
                return View(productVm);
            }
        }
        //Post method handler for Create Book, being passed a Book to work with
        [HttpPost]
        public async Task<IActionResult> Upsert(ProductVM vm, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (file is not null)
                {
                    await SaveImageAsync(vm, file);
                }

                if (vm.Product.ISBN is not null)
                {
                    vm.Product.ISBN = vm.Product.ISBN.Replace("-", string.Empty);
                }

                if (vm.Product.Id == 0)
                {
                    await UnitOfWork.ProductRepository.Add(vm);
                    TempData["success"] = $"Book {vm.Product.Title} created successfully!";
                }
                else
                {
                    await UnitOfWork.ProductRepository.Update(vm);
                    TempData["success"] = $"Book {vm.Product.Title} updated successfully!";
                }

                await UnitOfWork.SaveAsync();
                //Save to tempdata for accessing on next view w/ toastr.

                return RedirectToAction("Index", "Product");
            }
            else
            {
                var productVm = new ProductVM()
                {
                    CategoryList = UnitOfWork.CategoryRepository.GetAll().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }),
                    Product = new Product()
                };
                return View(productVm);
            }

        }

        private async Task SaveImageAsync(ProductVM vm, IFormFile file)
        {
            string filename = vm.Product.ImageURL ?? @"Images\Product\" + Guid.NewGuid().ToString();
            string path = Path.Combine(appEnvironment.WebRootPath, filename);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                filename = @"Images\Product\" + Guid.NewGuid().ToString();
                path = Path.Combine(appEnvironment.WebRootPath, filename);

            }

            using (var image = await Image.LoadAsync(file.OpenReadStream()))
            {
                if (image.Width != SD.CoverWidth || image.Height != SD.CoverHeight)
                {
                    image.Mutate(img => img.Resize(SD.CoverWidth, SD.CoverHeight, KnownResamplers.Lanczos3));
                }
                await image.SaveAsync(path, new JpegEncoder{Quality = 75});
            }

            vm.Product.ImageURL = filename;
        }

        /************************************
        DELETE PRODUCT
        ************************************/
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null or 0) //Null check for id
            {
                return NotFound();
            }
            Product productFromDb = await UnitOfWork.ProductRepository.Get(c => c.Id == id, includeOperators: "Categories") ?? new Product();

            return View(productFromDb);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Product toDelete)
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
            await UnitOfWork.SaveAsync();
            TempData["success"] = $"Book {toDelete.Title} removed successfully!";
            return RedirectToAction("Index", "Product");
        }

    }
}
