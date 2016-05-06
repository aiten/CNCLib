using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CNCLib.Web.MVC
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
		}
		protected void Application_BeginRequest()
		{
			var currentCulture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
			currentCulture.NumberFormat.NumberDecimalSeparator = ".";
			currentCulture.NumberFormat.NumberGroupSeparator = " ";
			currentCulture.NumberFormat.CurrencyDecimalSeparator = ".";

			Thread.CurrentThread.CurrentCulture = currentCulture;
			//Thread.CurrentThread.CurrentUICulture = currentCulture;
		}
	}
}
