using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using club.soundyard.web.Models;
using System.Data.Entity.Migrations;

namespace club.soundyard.web.Migrations
{

    internal sealed class Configuration : DbMigrationsConfiguration<club.soundyard.web.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(club.soundyard.web.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            // Vytvořit roli Admin, pokud ještě neexistuje
            if (!roleManager.RoleExists("Admin"))
            {
                var role = new ApplicationRole("Admin", "Administrator role");
                roleManager.Create(role);
            }

            // Vytvořit roli User, pokud ještě neexistuje
            if (!roleManager.RoleExists("User"))
            {
                var role = new ApplicationRole("User", "Standard user role");
                roleManager.Create(role);
            }

            // Přidání administrátora nebo jiného uživatele
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var adminUser = userManager.FindByEmail("admin@example.com");

            if (adminUser == null)
            {
                var user = new ApplicationUser { UserName = "admin@example.com", Email = "admin@example.com" };
                userManager.Create(user, "AdminPass123!");
                userManager.AddToRole(user.Id, "Admin");
            }
        }
    }
}
