using BookingApp.Data;
using BookingApp.Helpers;
using BookingApp.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Data
{
    public class DbInitializer
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public DbInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task Initialize()
        {
            var isCreated = context.Database.EnsureCreated();

            #region Identity init
            if (!await roleManager.RoleExistsAsync(RoleTypes.Admin))
                await roleManager.CreateAsync(new IdentityRole() { Name = RoleTypes.Admin });
            if (!await roleManager.RoleExistsAsync(RoleTypes.User))
                await roleManager.CreateAsync(new IdentityRole() { Name = RoleTypes.User });

            var user = new ApplicationUser()
            { Email = "admin@gmail.com", UserName = "Admin" };

            if (await userManager.FindByEmailAsync("admin@gmail.com") == null)
                await userManager.CreateAsync(user, "1qaz.2wsx"); //Temporary password
            if (!await userManager.IsInRoleAsync(user, RoleTypes.Admin))
                await userManager.AddToRoleAsync(user, RoleTypes.Admin);
            if (!await userManager.IsInRoleAsync(user, RoleTypes.User))
                await userManager.AddToRoleAsync(user, RoleTypes.User);
            #endregion
        }
    }
}
