using System.Web.Mvc;

namespace CNCLib.Web.MVC.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult About()
		{
			ViewBag.Message = "About CNCLib";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "CNCLib contact page.";

			return View();
		}
	}
}