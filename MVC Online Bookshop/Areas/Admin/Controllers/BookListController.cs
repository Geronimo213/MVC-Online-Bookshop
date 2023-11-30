using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;
using Bookshop.Models.ViewModels;
using Bookshop.Utility;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;

namespace MVC_Online_Bookshop.Areas.Admin.Controllers
{
    [Authorize(Roles = $"{SD.RoleAdmin}, {SD.RoleEmployee}")]
    [Area("Admin")]
    public class BookListController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public BookListController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index(int? pageNumber)
        {
            var lists = _unitOfWork.BookListRepository.GetAll().Include(bl => bl.Books).AsNoTracking();

            var paginatedLists = await PaginatedList<BookList>.CreateAsync(lists, pageNumber ?? 1, 25);
            return View(paginatedLists);
        }

        public async Task<IActionResult> Create(int? id)
        {
            var bookList = new BookList();
            if (id is not null or 0)
            {
                bookList = await _unitOfWork.BookListRepository.Get(x => x.Id == id);
            }

            return View(bookList);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookList list)
        {
            if (ModelState.IsValid)
            {
                if (list.Id is not 0)
                {
                    _unitOfWork.BookListRepository.Update(list);
                }
                else
                {
                    _unitOfWork.BookListRepository.Add(list);
                }
                
                await _unitOfWork.SaveAsync();

                return RedirectToAction(nameof(Edit), "BookList", new { list.Id });
            }
            TempData["error"] = "Model state invalid.";
            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewData["returnUri"] = Request.GetTypedHeaders().Referer;
            var list = await _unitOfWork.BookListRepository.Get(bl => bl.Id == id, includeOperators:"Books");
            if (list is null) return NotFound();
            var vm = new BookListVM()
            {
                List = list
            };
            return View(vm);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(BookListVM vm)
        {
            if (!ModelState.IsValid || vm.List.Id == 0) return View(vm);
            var listDb = await _unitOfWork.BookListRepository.Get(bl => bl.Id == vm.List.Id, includeOperators:"Books");
            var bookDb = await _unitOfWork.ProductRepository.Get(b => b.Id == vm.NewBookId);
            if (listDb is null || bookDb is null) return NotFound();

            if (!listDb.Books.Contains(bookDb))
            {
                listDb.Books.Add(bookDb);
                await _unitOfWork.SaveAsync();
                TempData["success"] = "Added book";
                return RedirectToAction(nameof(Edit), "BookList", new { vm.List.Id });
            }

            TempData["error"] = "List already contains book.";
            return View(vm);
        }

        public async Task<IActionResult> RemoveBook(int listId, int bookId)
        {
            var bookDb = await _unitOfWork.ProductRepository.Get(b => b.Id == bookId);
            var listDb = await _unitOfWork.BookListRepository.Get(bl => bl.Id == listId, includeOperators:"Books");
            if (bookDb is null || listDb is null) return NotFound("Could not find book and/or list.");

            listDb.Books.Remove(bookDb);
            await _unitOfWork.SaveAsync();

            TempData["success"] = "Removed book from list";

            return RedirectToAction(nameof(Edit), new {id = listId });
        }


        public async Task<IActionResult> RemoveList(int id)
        {
            var listDb = await _unitOfWork.BookListRepository.Get(bl => bl.Id == id);
            if (listDb is null) return NotFound("List to delete not found.");

            _unitOfWork.BookListRepository.Delete(listDb);
            await _unitOfWork.SaveAsync();

            TempData["success"] = $"removed list {listDb.Name}.";
            return RedirectToAction(nameof(Index));
        }


        [Route("/book-list")]
        public async Task<IActionResult> GetBooks(string q)
        {
            if (string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)) return null;

            var productQuery = _unitOfWork.ProductRepository.GetAll();

            var searchTerms = q.ToLower().Split(' ', ',', '.', ';', ':').Except(SD.StopWords);
            foreach (var term in searchTerms)
            {
                var productSearchPredicate = PredicateBuilder.New<Product>();
                productSearchPredicate = productSearchPredicate
                    .Or(x => x.Title.Contains(term))
                    .Or(x => x.Author!.Contains(term))
                    .Or(x => x.ISBN!.Contains(term));
                productQuery = productQuery.Where(productSearchPredicate);
            }

            var bookIds = await productQuery.Select(x => new BookIds() { Id = x.Id, Text = $"{x.Title} by {x.Author} - {x.ISBN}" }).ToListAsync();

            return Json(new { items = bookIds });
        }

        private class BookIds
        {
            public int Id { get; set; }
            public string Text { get; set; } = string.Empty;
        }
    }
}
