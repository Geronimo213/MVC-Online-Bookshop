using Bookshop.DataAccess.Data;
using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;
using Bookshop.Models.ViewModels;
using Bookshop.Utility;
using Microsoft.AspNetCore.Mvc;
using static System.Reflection.Metadata.BlobBuilder;
using System.Drawing.Printing;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;

namespace MVC_Online_Bookshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderManagementController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderManagementController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageSize, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["IdSortParam"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["NameSortParam"] = sortOrder == "Name" ? "name_desc" : "Name";
            ViewData["PlaceDateSortParam"] = sortOrder == "PlaceDate" ? "place_date_desc" : "PlaceDate";
            ViewData["ShipDateSortParam"] = sortOrder == "ShipDate" ? "ship_date_desc" : "ShipDate";
            ViewData["StatusSortParam"] = sortOrder == "Status" ? "status_desc" : "Status";

            if (searchString != null)
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


            List<OrderVM> orders = new List<OrderVM>();

            var orderQuery = _unitOfWork.OrderRepository.GetAll();
            orderQuery = String.IsNullOrEmpty(searchString) ? orderQuery : orderQuery.Where(s =>
                s.OrderId.ToString().Contains(searchString)
                || s.Name.Contains(searchString)
                || s.PlaceDate.ToString("MM/dd/yyyy").Contains(searchString)
                || s.BillState!.Contains(searchString)
                || s.BillCity!.Contains(searchString)
                );

            switch (sortOrder)
            {
                case "id_desc":
                    orderQuery = orderQuery.OrderByDescending(s => s.OrderId);
                    break;
                case "Name":
                    orderQuery = orderQuery.OrderBy(s => s.Name);
                    break;
                case "name_desc":
                    orderQuery = orderQuery.OrderByDescending(s => s.Name);
                    break;
                case "PlaceDate":
                    orderQuery = orderQuery.OrderBy(s => s.PlaceDate);
                    break;
                case "place_date_desc":
                    orderQuery = orderQuery.OrderByDescending(s => s.PlaceDate);
                    break;
                case "ShipDate":
                    orderQuery = orderQuery.OrderBy(s => s.ShipDate);
                    break;
                case "ship_date_desc":
                    orderQuery = orderQuery.OrderByDescending(s => s.ShipDate);
                    break;
                case "Status":
                    orderQuery = orderQuery.OrderBy(s => s.OrderStatus);
                    break;
                case "status_desc":
                    orderQuery = orderQuery.OrderByDescending(s => s.OrderStatus);
                    break;
                default:
                    orderQuery = orderQuery.OrderBy(s => s.OrderId);
                    break;
            }

            //var orderHeaders =
            //    await PaginatedList<Order>.CreateAsync(orderQuery.AsNoTracking(), pageNumber ?? 1, (int)pageSize);
            //var orderReports = new PaginatedList<OrderVM>(orderQuery.Count(), pageNumber ?? 1, (int)pageSize);
            //foreach (var order in orderHeaders)
            //{
            //    var orderLinesQuery = _unitOfWork.OrderLinesRepository.GetAll(x => x.OrderId == order.OrderId);
            //    var orderReport = new OrderVM();
            //    if (orderLinesQuery is not null)
            //    {
            //        orderReport.Header = order;
            //        orderReport.Lines = await orderLinesQuery.ToListAsync();

            //    }
            //    orderReports.Add(orderReport);
            //}

            var orderLinesQuery = _unitOfWork.OrderLinesRepository.GetAll();

            var orderReports = await
                PaginatedList<OrderVM>.CreateAsync(orderQuery, orderLinesQuery, pageNumber ?? 1, (int)pageSize);

            return View(orderReports);
        }
    }
}
