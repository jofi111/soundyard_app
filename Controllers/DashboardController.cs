using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using club.soundyard.web.Models;

public class DashboardController : Controller
{
    private readonly ApplicationDbContext db = new ApplicationDbContext();

    [Authorize(Roles = "User")]
    public ActionResult Index()
    {
        var userId = User.Identity.GetUserId();
        var userRoles = db.Users.Find(userId).Roles.Select(r => r.RoleId).ToList();
        var role = db.Roles.OfType<ApplicationRole>().FirstOrDefault(r => r.Id == userRoles.FirstOrDefault());
        var viewModel = new DashboardViewModel
        {
            Agreement = role?.Agreement ?? "No Agreement found"
        };

        return View();
    }
}
