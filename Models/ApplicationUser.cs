using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using System.Threading.Tasks;

namespace club.soundyard.web.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsEmailVerified { get; set; }

        // Přidání metody GenerateUserIdentityAsync
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Základní autentizace s použitím cookie autentifikace
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // Zde můžete přidat další vlastnosti do identity uživatele (claims)

            return userIdentity;
        }
    }
}
