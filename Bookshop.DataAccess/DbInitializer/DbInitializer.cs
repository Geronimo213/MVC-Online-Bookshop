using Bookshop.DataAccess.Data;
using Bookshop.Models;
using Bookshop.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Bookshop.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly AppDBContext _DbContext;
        private readonly UserManager<AppUser> _UserManager;
        private readonly RoleManager<IdentityRole> _RoleManager;
        private readonly IConfiguration _Configuration;

        public DbInitializer(AppDBContext db, UserManager<AppUser> um, RoleManager<IdentityRole> rm, IConfiguration cfg)
        {
            this._DbContext = db;
            this._UserManager = um;
            this._RoleManager = rm;
            this._Configuration = cfg;
        }
        public async Task<bool> Initialize()
        {
            //apply migrations if necessary
            //try
            //{
            //    if ((await _DbContext.Database.GetPendingMigrationsAsync()).Any())
            //    {
            //        await _DbContext.Database.MigrateAsync();
            //    }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //    return false;
            //}

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
                var pwd = _Configuration.GetValue<string>("AdminPassword");
                if (pwd is not null)
                {
                    await _UserManager.CreateAsync(new AppUser()
                    {
                        UserName = "admin@admin.com",
                        Email = "admin@admin.com",
                        Name = "Admin",
                    }, pwd);
                    var userDb = await _DbContext.AppUsers.FirstOrDefaultAsync(u => u.Email == "admin@admin.com") ??
                                 throw new Exception("Could not retrieve newly created user from DB.");
                    await _UserManager.AddToRoleAsync(userDb, SD.RoleAdmin);
                    var token = await _UserManager.GenerateEmailConfirmationTokenAsync(userDb);
                    var result = await _UserManager.ConfirmEmailAsync(userDb, token);
                    if (!result.Succeeded)
                    {
                        throw new Exception("Could not confirm default Admin account.");
                    }
                }
                else
                {
                    throw new Exception("Default admin password not found.");
                }
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
