using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;
using Bookshop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.EntityFrameworkCore;
using MVC_Online_Bookshop.ViewComponents;

namespace MVC_Online_Bookshop.Areas.Admin.Controllers
{
    [Area(SD.RoleAdmin)]
    [Authorize(Roles = $"{SD.RoleAdmin}, {SD.RoleEmployee}")]
    public class FrontPageController : Controller
    {
        private IUnitOfWork _unitOfWork;
        public FrontPageController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            var carouselsDb = _unitOfWork.CarouselRepository.GetAll().Include(x => x.Category).OrderBy(x => x.DisplayOrder);
            return View(await carouselsDb.ToListAsync());
        }

        public async Task<IActionResult> Upsert(int? id, Uri? returnUri)
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
        public async Task<IActionResult> Upsert(Carousel carousel, Uri? returnUri)
        {
            if (!ModelState.IsValid) return NotFound();

            if (carousel.Id is 0)
            {
                _unitOfWork.CarouselRepository.Add(carousel);
            }

            else
            {
                _unitOfWork.CarouselRepository.Update(carousel);
            }

            await _unitOfWork.SaveAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();
            var returnUri = Url.Action(nameof(Index));
            ViewData["ReturnUri"] = returnUri;

            var carouselDb = await _unitOfWork.CarouselRepository.Get(x => x.Id == id);
            if (carouselDb is null) return NotFound();

            return View(carouselDb);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Carousel carousel)
        {
            if (carousel.Id is 0)
            {
                return NotFound();
            }

            _unitOfWork.CarouselRepository.Delete(carousel);
            await _unitOfWork.SaveAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
