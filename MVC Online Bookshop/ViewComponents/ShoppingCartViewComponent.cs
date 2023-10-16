using Bookshop.DataAccess.Repository;
using Bookshop.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Bookshop.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MVC_Online_Bookshop.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private  IUnitOfWork UnitOfWork { get; }

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity?)User.Identity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if (claim is not null)
            {
                if (HttpContext.Session.GetInt32(SD.SessionCart) is null)
                {
                    HttpContext.Session.SetInt32(SD.SessionCart,
                        await UnitOfWork.ShoppingCartRepository.GetAll().CountAsync(x => x.UserId == claim.Value));
                }
                
                return View((int)HttpContext.Session.GetInt32(SD.SessionCart)!);
            }

            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
