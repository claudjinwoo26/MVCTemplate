using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MVCtemplate.DataAccess.Data;
using MVCTemplate.Models;
using MVCTemplate.Util;

namespace MVCTemplate.DataAccess.Data;

public class Seed
{
    public static void SeedData(IApplicationBuilder applicationBuilder) 
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope()) 
        {
            var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            context.Database.EnsureCreated();
        }
    }

    public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder) 
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope()) 
        {
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync(Roles.Admin)) 
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
            }
            if (!await roleManager.RoleExistsAsync(Roles.User)) 
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.User));
            }

            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            //Sample email, adjsut as needed
            var adminEmail = "admin@template.com";
            var userEmail = "user@template.com";


            //Seeding of admin account
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser is null) 
            {
                var newAdminUser = new ApplicationUser()
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(newAdminUser, Roles.Default_Password);
                await userManager.AddToRoleAsync(newAdminUser, Roles.Admin);
            }
            var user = await userManager.FindByEmailAsync(userEmail);
            if (user is null) 
            {
                var newUser = new ApplicationUser()
                {
                    UserName = userEmail,
                    Email = userEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(newUser, Roles.Default_Password);
                await userManager.AddToRoleAsync(newUser, Roles.User);
            }
          

        }
    }
}
