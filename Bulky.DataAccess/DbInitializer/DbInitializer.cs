using Bulky.Models.Models;
using Bulky.Utility;
using BulkyWeb.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManger;
        private readonly RoleManager<IdentityRole> _roleManger;
        private readonly ApplicationDbContext _db;

        public DbInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
        { 
            _roleManger = roleManager;
            _userManger = userManager;
            _db = db;
        }
        public void Initialize()
        {
            //migrations if they are not applied
            try
            {
                if (_db.Database.GetPendingMigrations().Count()>0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            //Create roles if they are not created
            //To add role in Db side
            if (!_roleManger.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManger.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManger.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                _roleManger.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManger.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();

                //If roles are not created, then we will crete admin user as well
                _userManger.CreateAsync(new ApplicationUser
                {
                    UserName = "atharvumak72@gmail.com",
                    Email = "atharvumak72@gmail.com",
                    Name = "Atharv Umak",
                    PhoneNumber = "8668453402",
                    StreetAddress = "Manish Nagar Nagpur",
                    State = "Maharashtra",
                    PostalCode = "440001",
                    City = "Nagpur"
                }, "Atharv@123").GetAwaiter().GetResult();

                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u=>u.Email== "atharvumak72@gmail.com");
                _userManger.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();

            }
            return;
        }
    }
}
