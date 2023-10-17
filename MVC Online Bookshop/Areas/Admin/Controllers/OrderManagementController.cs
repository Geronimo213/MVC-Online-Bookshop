﻿using Bookshop.DataAccess.Data;
using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;
using Bookshop.Models.ViewModels;
using Bookshop.Utility;
using Microsoft.AspNetCore.Mvc;
using static System.Reflection.Metadata.BlobBuilder;
using System.Drawing.Printing;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Build.Framework;
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
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageSize, int? pageNumber)
        {

            ViewData["CurrentSort"] = sortOrder;
            ViewData["IdSortParam"] = String.IsNullOrEmpty(sortOrder) ? "id_asc" : "";
            ViewData["NameSortParam"] = sortOrder == "Name" ? "name_desc" : "Name";
            ViewData["PlaceDateSortParam"] = sortOrder == "PlaceDate" ? "place_date_desc" : "PlaceDate";
            ViewData["ShipDateSortParam"] = sortOrder == "ShipDate" ? "ship_date_desc" : "ShipDate";
            ViewData["StatusSortParam"] = sortOrder == "Status" ? "status_desc" : "Status";

            if (!String.IsNullOrEmpty(searchString))
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
            DateTime.TryParse(searchString, out DateTime searchDate);
            var orderQuery = _unitOfWork.OrderRepository.GetAll();
            orderQuery = String.IsNullOrEmpty(searchString) ? orderQuery : orderQuery.Where(s =>
                s.OrderId.ToString().Contains(searchString)
                || s.Name.Contains(searchString)
                || s.PlaceDate.Date == searchDate.Date
                || s.BillState!.Contains(searchString)
                || s.BillCity!.Contains(searchString)
                );

            switch (sortOrder)
            {
                case "id_asc":
                    orderQuery = orderQuery.OrderBy(s => s.OrderId);
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
                    orderQuery = orderQuery.OrderByDescending(s => s.OrderId);
                    break;
            }



            var orderLinesQuery = _unitOfWork.OrderLinesRepository.GetAll();

            var orderReports = await
                PaginatedList<OrderVM>.CreateAsync(orderQuery, orderLinesQuery, pageNumber ?? 1, (int)pageSize, includeOperators:"Product");

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
                    Header = await _unitOfWork.OrderRepository.Get(x => x.OrderId == orderId) ?? new Order(),
                    Lines = await _unitOfWork.OrderLinesRepository.GetAll(x => x.OrderId == orderId, includeOperators: "Product")!.ToListAsync()
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
                order.Header.OrderStatus = "Shipped";
                order.Header.ShipDate = DateTime.Now;
                
                TempData["success"] = "Tracking updated!";
            }
            _unitOfWork.OrderRepository.Update(order.Header);
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(OrderDetails), new {orderId = order.Header.OrderId, returnUri = returnUri});
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

            if (returnUri is not null)
            {
                return LocalRedirect(returnUri.LocalPath + returnUri.Query);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
