using Bookshop.DataAccess.Data;
using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;

namespace Bookshop.DataAccess.Repository
{
    public class AppUserRepository : Repository<AppUser>, IAppUserRepository
    {
        private readonly AppDBContext _dbContext;
        public AppUserRepository(AppDBContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }

        public IQueryable<AppUser> GetAllWithRoles()
        {
            var query = (from user in DbSet
                         join ur in _dbContext.UserRoles on user.Id equals ur.UserId
                         join r in _dbContext.Roles on ur.RoleId equals r.Id
                         select new AppUser()
                         {
                             Id = user.Id,
                             Name = user.Name,
                             Email = user.Email,
                             NormalizedEmail = user.NormalizedEmail,
                             City = user.City,
                             State = user.State,
                             PostalCode = user.PostalCode,
                             LockoutEnabled = user.LockoutEnabled,
                             LockoutEnd = user.LockoutEnd,
                             PhoneNumber = user.PhoneNumber,
                             StreetAddress = user.StreetAddress,
                             UserName = user.UserName,
                             NormalizedUserName = user.NormalizedUserName,
                             Role = r.Name
                         });
            return query;
        }

        //public void Update(AppUser appUser)
        //{
        //    _dbContext.AppUsers.Update(appUser);
        //}
    }
}
