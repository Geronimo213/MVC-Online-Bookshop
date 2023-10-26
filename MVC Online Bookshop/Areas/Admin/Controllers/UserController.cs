using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;
using Bookshop.Models.ViewModels;
using Bookshop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using LinqKit;

namespace MVC_Online_Bookshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.RoleAdmin)]
    public class UserController : Controller
    {
        private IUnitOfWork UnitOfWork { get; }
        private IWebHostEnvironment AppEnvironment { get; set; }
        private UserManager<IdentityUser> AppUserManager { get; set; }
        private RoleManager<IdentityRole> RoleManager { get; set; }

        public UserController(IUnitOfWork unitOfWork, IWebHostEnvironment appEnvironment, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> appUserManager)
        {
            this.UnitOfWork = unitOfWork;
            this.AppEnvironment = appEnvironment;
            this.AppUserManager = appUserManager;
            this.RoleManager = roleManager;
        }


        /************************************
        RECEIVE USERS (INDEX)
        ************************************/

        //Handler for main Category page
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageSize, int? pageNumber, PaginatedUserVM? vm)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["IdSortParam"] = string.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["NameSortParam"] = sortOrder == "Name" ? "name_desc" : "Name";
            ViewData["RoleSortParam"] = sortOrder == "Role" ? "role_desc" : "Role";

            if (!string.IsNullOrEmpty(searchString))
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

            var users = UnitOfWork.AppUserRepository.GetAllWithRoles(); //Custom linq query joining USERS, USER_ROLES, ROLES, selecting most user columns and roles
            var rolesFromDb =
                (await RoleManager.Roles.ToListAsync()).Select(x => new SelectListItem(text: x.Name, value: x.Name)).ToList();

            vm ??= new PaginatedUserVM();
            if (vm.SelectedRoles.Count < 1)
            {
                foreach (var selectListItem in rolesFromDb.Where(x => x.Value != "Customer"))
                {
                    vm.SelectedRoles.Add(selectListItem.Value);
                }
            }

            var rolePredicate = PredicateBuilder.New<AppUser>();
            foreach (var selectedRole in vm.SelectedRoles)
            {
                rolePredicate = rolePredicate.Or(p => p.Role == selectedRole);
            }

            users = users.Where(rolePredicate);

            users = string.IsNullOrEmpty(searchString) ? users : users.Where(s =>
                s.Name.Contains(searchString)
                || (s.NormalizedEmail != null && s.NormalizedEmail.Contains(searchString))
                || (s.State != null && s.State.Contains(searchString))
                || (s.City != null && s.City.Contains(searchString))
                || (s.Role != null && s.Role.Contains(searchString))
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



            var paginatedUsers = await PaginatedList<AppUser>.CreateAsync(users, pageNumber ?? 1, (int)pageSize);

            vm.Users = paginatedUsers;
            vm.RoleList = rolesFromDb;
            //int pageSize = SD.PageSizeProduct;
            return View(vm);
        }

        /************************************
        UPDATE PERMISSIONS
        ************************************/

        public async Task<IActionResult> EditPermissions(string? userId, Uri? returnUri)
        {
            if (userId == null) return NotFound();

            returnUri ??= HttpContext.Request.GetTypedHeaders().Referer;
            ViewData["ReturnUri"] = returnUri;

            var userFromDb = await UnitOfWork.AppUserRepository.Get(x => x.Id == userId);
            if (userFromDb == null) return NotFound();
            userFromDb.Role = (await AppUserManager.GetRolesAsync(userFromDb)).FirstOrDefault();
            var rolesFromDb = (await RoleManager.Roles.ToListAsync()).Select(x => new SelectListItem(text: x.Name, value: x.Name));
            var user = new UserVM()
            {
                User = userFromDb,
                RoleList = rolesFromDb
            };


            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditPermissions(UserVM vm, Uri? returnUri)
        {
            ViewData["ReturnUri"] = returnUri;

            var userFromDb = await UnitOfWork.AppUserRepository.Get(x => x.Id == vm.User.Id, tracked: true);
            if (userFromDb == null) { return NotFound(); }

            var originalRole = (await AppUserManager.GetRolesAsync(userFromDb)).FirstOrDefault();

            if (originalRole == vm.User.Role ||
                vm.User.Role is null || originalRole is null)
            {
                TempData["error"] = $"Error! Please be sure to select a role!";
                return RedirectToAction(nameof(EditPermissions), new { returnUri });
            }

            await AppUserManager.RemoveFromRoleAsync(userFromDb, originalRole);
            await UnitOfWork.SaveAsync();
            await AppUserManager.AddToRoleAsync(userFromDb, vm.User.Role);
            await UnitOfWork.SaveAsync();

            TempData["success"] =
                $"Successfully changed role from {originalRole} to {vm.User.Role} for user {userFromDb.Name}";
            return RedirectToAction(nameof(EditPermissions), new { returnUri });

        }

        /************************************
        LOCK/UNLOCK USER
        ************************************/
        public async Task<IActionResult> ToggleLock(string? userId, Uri? returnUri)
        {

            returnUri ??= HttpContext.Request.GetTypedHeaders().Referer;
            if (userId == null || userId == User.FindFirst(ClaimTypes.NameIdentifier)!.Value)
            {
                TempData["warning"] = "Cannot find user with given ID. You cannot lock the current user.";
                return returnUri is not null ? LocalRedirect(returnUri.LocalPath + returnUri.Query) : RedirectToAction(nameof(Index));
            }

            var user = await UnitOfWork.AppUserRepository.Get(u => u.Id == userId);
            if (user == null) { return NotFound(); }

            user.LockoutEnd = user.LockoutEnd > DateTime.Now ? null : DateTime.Now.AddYears(1000);

            await UnitOfWork.SaveAsync();
            if (returnUri is not null)
            {
                return LocalRedirect(returnUri.LocalPath + returnUri.Query);
            }
            return RedirectToAction(nameof(Index));
        }


        /************************************
        REMOVE USER
        ************************************/
        public async Task<IActionResult> RemoveUser(string? userId)
        {
            var returnUri = HttpContext.Request.GetTypedHeaders().Referer;
            ViewData["ReturnUri"] = returnUri;

            if (userId == null || userId == User.FindFirst(ClaimTypes.NameIdentifier)!.Value)
            {
                TempData["warning"] = "Cannot find user with given ID. You cannot remove the current user.";
                return returnUri is not null ? LocalRedirect(returnUri.LocalPath + returnUri.Query) : RedirectToAction(nameof(Index));
            }

            var userFromDb = await UnitOfWork.AppUserRepository.Get(x => x.Id == userId);
            if (userFromDb == null) return NotFound();
            userFromDb.Role = (await AppUserManager.GetRolesAsync(userFromDb)).FirstOrDefault();


            return View(userFromDb);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveUser(string? userId, Uri? returnUri)
        {
            if (userId is null)
            {
                return NotFound();
            }

            var userFromDb = await UnitOfWork.AppUserRepository.Get(x => x.Id == userId, tracked: true);
            if (userFromDb == null) { return NotFound(); }

            await AppUserManager.DeleteAsync(userFromDb);
            await UnitOfWork.SaveAsync();

            if (returnUri is not null)
            {
                return LocalRedirect(returnUri.LocalPath + returnUri.Query);
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
