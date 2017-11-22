using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;
using Unity.AspNet.WebApi;

namespace CNCLib.WebAPI
{
    public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
            // Web API configuration and services

            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.DependencyResolver = new UnityDependencyResolver(UnityConfig.GetConfiguredContainer());

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
