using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;
using Bookshop.Models.ViewModels;
using Bookshop.Utility;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MVC_Online_Bookshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class OrderManagementController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderManagementController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageSize, int? pageNumber, string? statusFilter)
        {

            ViewData["CurrentSort"] = sortOrder;
            ViewData["IdSortParam"] = String.IsNullOrEmpty(sortOrder) ? "id_asc" : "";
            ViewData["NameSortParam"] = sortOrder == "Name" ? "name_desc" : "Name";
            ViewData["PlaceDateSortParam"] = sortOrder == "PlaceDate" ? "place_date_desc" : "PlaceDate";
            ViewData["ShipDateSortParam"] = sortOrder == "ShipDate" ? "ship_date_desc" : "ShipDate";
            ViewData["StatusSortParam"] = sortOrder == "Status" ? "status_desc" : "Status";
            ViewData["OrderStatuses"] = new List<SelectListItem>()
            {
                new() {Text = "All", Value = string.Empty, Selected = string.IsNullOrEmpty(statusFilter)},
                new() {Text = SD.OrderPlaced, Value = SD.OrderPlaced, Selected = statusFilter == SD.OrderPlaced},
                new() {Text = SD.PaymentPending, Value = SD.PaymentPending, Selected = statusFilter == SD.PaymentPending},
                new() {Text = SD.PaymentProcessed, Value = SD.PaymentProcessed, Selected = statusFilter == SD.PaymentProcessed},
                new() {Text = SD.OrderShipped, Value = SD.OrderShipped, Selected = statusFilter == SD.OrderShipped}
            };

            if (!String.IsNullOrEmpty(searchString))
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter ?? string.Empty;
            }
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentStatus"] = statusFilter ?? string.Empty;

            pageSize ??= SD.PageSizeProduct;
            ViewData["CurrentPageSize"] = (int)pageSize;


            var orderQuery = _unitOfWork.OrderRepository.GetAll().AsNoTracking();

            var searchTerms = searchString.ToLower().Split(' ', ',', '.', ';', ':').Except(SD.StopWords);
            foreach (var term in searchTerms)
            {
                DateTime.TryParse(term, out DateTime searchDate);
                var orderSearchPredicate = PredicateBuilder.New<Order>();
                orderSearchPredicate = orderSearchPredicate
                    .Or(x => x.OrderId.ToString().Contains(term))
                    .Or(x => x.Name!.Contains(term))
                    .Or(x => x.PlaceDate.Date == searchDate.Date)
                    .Or(x => x.BillState!.Contains(term))
                    .Or(x => x.BillCity!.Contains(term));
                orderQuery = orderQuery.Where(orderSearchPredicate);
            }

            if (await orderQuery.AnyAsync())
            {
                orderQuery = statusFilter switch
                {
                    SD.OrderPlaced => orderQuery.Where(x => x.OrderStatus == SD.OrderPlaced),
                    SD.PaymentPending => orderQuery.Where(x => x.OrderStatus == SD.PaymentPending),
                    SD.PaymentProcessed => orderQuery.Where(x => x.OrderStatus == SD.PaymentProcessed),
                    SD.OrderShipped => orderQuery.Where(x => x.OrderStatus == SD.OrderShipped),
                    _ => orderQuery
                };

            }

            if (await orderQuery.AnyAsync())
            {
                orderQuery = sortOrder switch
                {
                    "id_asc" => orderQuery.OrderBy(s => s.OrderId),
                    "Name" => orderQuery.OrderBy(s => s.Name),
                    "name_desc" => orderQuery.OrderByDescending(s => s.Name),
                    "PlaceDate" => orderQuery.OrderBy(s => s.PlaceDate),
                    "place_date_desc" => orderQuery.OrderByDescending(s => s.PlaceDate),
                    "ShipDate" => orderQuery.OrderBy(s => s.ShipDate),
                    "ship_date_desc" => orderQuery.OrderByDescending(s => s.ShipDate),
                    "Status" => orderQuery.OrderBy(s => s.OrderStatus),
                    "status_desc" => orderQuery.OrderByDescending(s => s.OrderStatus),
                    _ => orderQuery.OrderByDescending(s => s.OrderId)
                };
            }
           

            var orderLinesQuery = _unitOfWork.OrderLinesRepository.GetAll().AsNoTracking();

            var orderReports = await
                PaginatedOrders.CreateAsync(orderQuery, orderLinesQuery, pageNumber ?? 1, (int)pageSize, includeOperators: "Product");

            return View(orderReports);
        }

        public async Task<IActionResult> OrderDetails(int? orderId, Uri? returnUri)
        {
            returnUri ??= HttpContext.Request.GetTypedHeaders().Referer;
            ViewData["ReturnUri"] = returnUri;
            if (orderId is not null)
            {
                var order = new OrderVM()
                {
                    Header = await _unitOfWork.OrderRepository.Get(x => x.OrderId == orderId, tracked:false) ?? new Order(),
                    Lines = await _unitOfWork.OrderLinesRepository.GetAll(x => x.OrderId == orderId, includeOperators: "Product")!.AsNoTracking().ToListAsync()
                };
                return View(order);
            }
            TempData["error"] = "Order not found.";
            return NotFound();
        }

        public async Task<IActionResult> UpdateShipping(OrderVM order, Uri? returnUri)
        {
            if (order.Header.OrderId != 0 && order.Header.TrackingNumber != null)
            {
                order.Header.OrderStatus = SD.OrderShipped;
                order.Header.ShipDate = DateTime.Now;

                TempData["success"] = "Tracking updated!";
            }
            _unitOfWork.OrderRepository.Update(order.Header);
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(OrderDetails), new { orderId = order.Header.OrderId, returnUri = returnUri });
        }

        public async Task<IActionResult> RemoveOrder(int? orderId)
        {
            var returnUri = HttpContext.Request.GetTypedHeaders().Referer;
            ViewData["ReturnUri"] = returnUri;

            if (orderId is null)
            {
                return returnUri is null ? RedirectToAction(nameof(Index)) : Redirect(returnUri.ToString()); //Update to use local path
            }
            var order = new OrderVM()
            {
                Header = await _unitOfWork.OrderRepository.Get(x => x.OrderId == orderId) ?? new Order(),
                Lines = await _unitOfWork.OrderLinesRepository.GetAll(x => x.OrderId == orderId, includeOperators: "Product")!.ToListAsync()
            };


            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveOrder(OrderVM order, Uri? returnUri)
        {
            var linesToDelete = await _unitOfWork.OrderLinesRepository.GetAll(x => x.OrderId == order.Header.OrderId)!.ToListAsync();
            _unitOfWork.OrderLinesRepository.DeleteRange(order.Lines);
            _unitOfWork.OrderRepository.Delete(order.Header);
            await _unitOfWork.SaveAsync();

            TempData["success"] = $"Removed order {order.Header.OrderId}";

            if (returnUri is not null)
            {
                return LocalRedirect(returnUri.LocalPath + returnUri.Query);
            }


            return RedirectToAction(nameof(Index));
        }
    }
}
