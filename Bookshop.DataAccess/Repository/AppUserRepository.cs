using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bookshop.DataAccess.Data;
using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookshop.DataAccess.Repository
{
    public class AppUserRepository : Repository<AppUser>, IAppUserRepository
    {
        private readonly AppDBContext _dbContext;
        public AppUserRepository(AppDBContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }

        //public void Update(AppUser appUser)
        //{
        //    _dbContext.AppUsers.Update(appUser);
        //}
    }
}
