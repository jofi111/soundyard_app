using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace club.soundyard.web.Models
{
    public class ApplicationRole : IdentityRole
    {
        [StringLength(50)]
        public string Agreement { get; set; }
        public ApplicationRole() : base() { }

        public ApplicationRole(string roleName, string agreement) : base(roleName)
        {
            this.Agreement = agreement;
        }
    }
}
