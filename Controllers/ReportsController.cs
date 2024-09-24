using System.Web.Mvc;

namespace club.soundyard.web.Controllers
{
    public class ReportsController : Controller
    {
        [Authorize(Roles = "User")]
        public ActionResult Index()
        {
            return View();
        }
    }
}