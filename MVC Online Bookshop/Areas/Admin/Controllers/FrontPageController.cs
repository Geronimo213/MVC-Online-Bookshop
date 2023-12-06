using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;
using Bookshop.Models.ViewModels;
using Bookshop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace MVC_Online_Bookshop.Areas.Admin.Controllers
{
    [Area(SD.RoleAdmin)]
    [Authorize(Roles = $"{SD.RoleAdmin}, {SD.RoleEmployee}")]
    public class FrontPageController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IWebHostEnvironment appEnvironment { get; }
        public FrontPageController(IUnitOfWork unitOfWork, IWebHostEnvironment appEnvironment)
        {
            this._unitOfWork = unitOfWork;
            this.appEnvironment = appEnvironment;
        }

        //INDEX
        public async Task<IActionResult> Index()
        {
            var vm = new FrontPageManageVM();
            var carouselsDb = _unitOfWork.CarouselRepository.GetAll().Include(x => x.Category).OrderBy(x => x.DisplayOrder);
            var headersDb = await _unitOfWork.HeaderRepository.GetAll().ToListAsync();
            vm.Carousels = await carouselsDb.ToListAsync();
            vm.Headers = headersDb;
            return View(vm);
        }


        // HEADER OPERATIONS
        public async Task<IActionResult> HeaderUpsert(int? id, Uri? returnUri)
        {
            returnUri ??= HttpContext.Request.GetTypedHeaders().Referer;
            ViewData["ReturnUri"] = returnUri;
            if (id is null or 0)
            {
                return View(new Header());
            }

            var headerDb = await _unitOfWork.HeaderRepository.Get(x => x.Id == id);

            return View(headerDb);
        }

        [HttpPost]
        public async Task<IActionResult> HeaderUpsert(Header header, IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Model state is invalid.";
                return View(header);
            }
            if (file is not null)
            {
                await SaveBookImage(header, file);
            }

            if (header.Id is 0)
            {
                _unitOfWork.HeaderRepository.Add(header);
                TempData["success"] = "Created Slide";
            }
            else
            {
                _unitOfWork.HeaderRepository.Update(header);
                TempData["success"] = "Updated Slide";
            }

            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));

        }

        private async Task SaveBookImage(Header header, IFormFile file)
        {
            string filename = header.ImagePath ?? @"Images\Slides\" + Guid.NewGuid().ToString() + ".jpg";
            string path = Path.Combine(appEnvironment.WebRootPath, filename);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                filename = @"Images\Product\" + Guid.NewGuid().ToString() + ".jpg";
                path = Path.Combine(appEnvironment.WebRootPath, filename);
            }

            using (var image = await Image.LoadAsync(file.OpenReadStream()))
            {
                if (image.Width != 2048 || image.Height != 600)
                {
                    image.Mutate(img => img.Resize(2048, 600, KnownResamplers.Lanczos3));
                }
                await image.SaveAsync(path, new JpegEncoder{Quality = 75});
            }

            header.ImagePath = filename;
        }

        public async Task<IActionResult> HeaderDelete(int? id, Uri? returnUri)
        {
            returnUri ??= HttpContext.Request.GetTypedHeaders().Referer;
            ViewData["ReturnUri"] = returnUri;

            if (id is null or 0)
            {
                return NotFound();
            }

            var headerDb = await _unitOfWork.HeaderRepository.Get(x => x.Id == id);
            return View(headerDb);
        }
        [HttpPost]
        public async Task<IActionResult> HeaderDelete(Header header)
        {
            if (header.Id is 0)
            {
                return NotFound();
            }

            if (header.ImagePath is not null)
            {
                string filename = header.ImagePath;
                string path = Path.Combine(appEnvironment.WebRootPath, filename);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);

                }
            }

            _unitOfWork.HeaderRepository.Delete(header);
            await _unitOfWork.SaveAsync();

            TempData["success"] = "Removed slide";
            return RedirectToAction(nameof(Index));
        }


        //CAROUSEL OPERATIONS
        public async Task<IActionResult> CarouselUpsert(int? id, Uri? returnUri)
        {
            returnUri ??= HttpContext.Request.GetTypedHeaders().Referer;
            ViewData["ReturnUri"] = returnUri;
            ViewData["CategoryItems"] = await _unitOfWork.CategoryRepository.GetAll().OrderBy(x => x.DisplayOrder).Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToListAsync();
            ViewData["SortItems"] = new List<SelectListItem>()
            {
                new() { Text = "Title", Value = "Title" },
                new () { Text = "Author", Value = "Author" },
                new () { Text = "Recent", Value = "Id" }
            };

            if (id is null) return View(new Carousel());

            var carouselDb = await _unitOfWork.CarouselRepository.Get(x => x.Id == id);
            return View(carouselDb);


        }

        [HttpPost]
        public async Task<IActionResult> CarouselUpsert(Carousel carousel, Uri? returnUri)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(Environment.NewLine, ModelState.Keys.Where(i => ModelState[i]?.Errors.Count > 0).Select(k =>
                    new KeyValuePair<string, string>(k, ModelState[k]!.Errors.First().ErrorMessage)));
                TempData["error"] = $"Model state is invalid. Error log: {errors}";
                return View(carousel);
            }

            if (carousel.Id is 0)
            {
                _unitOfWork.CarouselRepository.Add(carousel);
                TempData["success"] = "Created carousel";
            }

            else
            {
                _unitOfWork.CarouselRepository.Update(carousel);
                TempData["success"] = "Updated carousel";
            }

            await _unitOfWork.SaveAsync();


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CarouselDelete(int? id)
        {
            if (id is null) return NotFound();
            var returnUri = Url.Action(nameof(Index));
            ViewData["ReturnUri"] = returnUri;

            var carouselDb = await _unitOfWork.CarouselRepository.Get(x => x.Id == id);
            if (carouselDb is null) return NotFound();

            return View(carouselDb);
        }

        [HttpPost]
        public async Task<IActionResult> CarouselDelete(Carousel carousel)
        {
            if (carousel.Id is 0)
            {
                return NotFound();
            }

            _unitOfWork.CarouselRepository.Delete(carousel);
            await _unitOfWork.SaveAsync();

            TempData["success"] = "Removed carousel";
            return RedirectToAction(nameof(Index));
        }
    }
}
