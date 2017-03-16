using System.Web.Http;

namespace CNCLib.WebAPI
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services

			// Web API routes
			config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);
/*
			// Controllers with Actions
			// To handle routes like `/api/VTRouting/route`
			config.Routes.MapHttpRoute(
				name: "ControllerAndAction",
				routeTemplate: "api/{controller}/{action}"
			);
*/
		}
	}
}
