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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace MVC_Online_Bookshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.RoleAdmin)]
    public class UserController : Controller
    {
        private IUnitOfWork UnitOfWork { get; }
        private IWebHostEnvironment appEnvironment { get; set; }

        public UserController(IUnitOfWork unitOfWork, IWebHostEnvironment appEnvironment)
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
            ViewData["IdSortParam"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["NameSortParam"] = sortOrder == "Name" ? "name_desc" : "Name";
            ViewData["RoleSortParam"] = sortOrder == "Role" ? "role_desc" : "Role";

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



            var users = (from user in UnitOfWork.AppUserRepository.GetAll()
                join ur in UnitOfWork.DbContext.UserRoles on user.Id equals ur.UserId
                join r in UnitOfWork.DbContext.Roles on ur.RoleId equals r.Id
                select new AppUser()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Role = r.Name,
                    Email = user.Email,
                    City = user.City,
                    State = user.State,
                    PostalCode = user.PostalCode,
                    NormalizedEmail = user.NormalizedEmail,
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEnd = user.LockoutEnd,
                    PhoneNumber = user.PhoneNumber,
                    StreetAddress = user.StreetAddress
                });
            
            
            users = string.IsNullOrEmpty(searchString) ? users : users.Where(s =>
                s.Name.Contains(searchString)
                || (s.NormalizedEmail != null && s.NormalizedEmail.Contains(searchString))
                || (s.State != null && s.State.Contains(searchString))
                || (s.City != null && s.City.Contains(searchString))
                || (s.Role!= null && s.Role.Contains(searchString))
                );

            users = sortOrder switch
            {
                "id_desc" => users.OrderByDescending(s => s.Id),
                "Name" => users.OrderBy(s => s.Name),
                "name_desc" => users.OrderByDescending(s => s.Name),
                "Role" => users.OrderBy(s => s.Role),
                "role_desc" => users.OrderByDescending(s => s.Role),
                _ => users.OrderBy(s => s.Id),
            };


            //int pageSize = SD.PageSizeProduct;
            return View(await PaginatedList<AppUser>.CreateAsync(users, pageNumber ?? 1, (int)pageSize));
        }

        /************************************
        CREATE OR UPDATE PRODUCT
        ************************************/
        //Get result for Create Book page
        public async Task<IActionResult> Upsert(int? id)
        {
            return NotFound();
        }
        //Post method handler for Create Book, being passed a Book to work with
        [HttpPost]
        public async Task<IActionResult> Upsert(ProductVM vm, IFormFile? file)
        {
            return NotFound();

        }





        /************************************
        DELETE PRODUCT
        ************************************/
        public async Task<IActionResult> Delete(int? id)
        {
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            return NotFound();
        }

    }
}
