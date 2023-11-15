using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bookshop.DataAccess.Data;
using Bookshop.Models;
using Bookshop.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bookshop.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly AppDBContext _DbContext;
        private readonly UserManager<AppUser> _UserManager;
        private readonly RoleManager<IdentityRole> _RoleManager;

        public DbInitializer(AppDBContext db, UserManager<AppUser> um, RoleManager<IdentityRole> rm)
        {
            this._DbContext = db;
            this._UserManager = um;
            this._RoleManager = rm;
        }
        public async Task<bool> Initialize()
        {
            //apply migrations if necessary
            try
            {
                if ((await _DbContext.Database.GetPendingMigrationsAsync()).Any())
                {
                    await _DbContext.Database.MigrateAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            //create roles if necessary
            if (await _RoleManager.RoleExistsAsync(SD.RoleAdmin)) return true;

            try
            {
                await _RoleManager.CreateAsync(new IdentityRole(SD.RoleAdmin));
                await _RoleManager.CreateAsync(new IdentityRole(SD.RoleCustomer));
                await _RoleManager.CreateAsync(new IdentityRole(SD.RoleEmployee));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            try
            {
                //create admin user if roles needed creating
                await _UserManager.CreateAsync(new AppUser()
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com",
                    Name = "Admin",
                }, "Admin*123");
                var userDb = await _DbContext.AppUsers.FirstOrDefaultAsync(u => u.Email == "admin@admin.com") ?? throw new Exception("Could not retrieve newly created user from DB.");
                await _UserManager.AddToRoleAsync(userDb, SD.RoleAdmin);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

           

            return true;
        }
    }
}
