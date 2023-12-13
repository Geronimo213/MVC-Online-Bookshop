using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MVC_Online_Bookshop.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private IUnitOfWork UnitOfWork { get; }
        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity?)User.Identity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            var cartCookie = Request.Cookies[SD.ShoppingCartCookie];


            if (claim is not null)
            {

                HttpContext.Session.SetInt32(SD.SessionCart,
                    await UnitOfWork.ShoppingCartRepository.GetAll().CountAsync(x => x.UserId == claim.Value));


                return View((int)HttpContext.Session.GetInt32(SD.SessionCart)!);
            }

            if (cartCookie is not null)
            {
                if (HttpContext.Session.GetInt32(SD.SessionCart) is null)
                {
                    HttpContext.Session.SetInt32(SD.SessionCart,
                        await UnitOfWork.ShoppingCartRepository.GetAll().CountAsync(x => x.SessionId == cartCookie));
                }

                return View((int)HttpContext.Session.GetInt32(SD.SessionCart)!);
            }

            HttpContext.Session.Clear();
            return View(0);

        }
    }
}
