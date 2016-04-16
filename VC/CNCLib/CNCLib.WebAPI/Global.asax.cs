using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Framework.Tools.Dependency;
using Framework.Tools.Pattern;
using Framework.EF;
using CNCLib.Repository.Context;

namespace CNCLib.WebAPI
{
	public class WebApiApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			Dependency.Initialize(new LiveDependencyProvider());
			Dependency.Container.RegisterTypesIncludingInternals(typeof(CNCLib.Repository.MachineRepository).Assembly, typeof(CNCLib.Logic.MachineController).Assembly);
			Dependency.Container.RegisterType<IUnitOfWork, UnitOfWork<CNCLibContext>>();

			AreaRegistration.RegisterAllAreas();
			GlobalConfiguration.Configure(WebApiConfig.Register);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}
	}
}
