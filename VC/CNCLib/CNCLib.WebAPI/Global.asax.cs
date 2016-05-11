using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Framework.Tools.Dependency;
using Framework.Tools.Pattern;
using Framework.EF;
using CNCLib.Repository.Context;
using AutoMapper;

namespace CNCLib.WebAPI
{
	public class WebApiApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			Dependency.Initialize(new LiveDependencyProvider());
			Dependency.Container.RegisterTypesIncludingInternals(typeof(CNCLib.Repository.MachineRepository).Assembly, typeof(CNCLib.Logic.MachineController).Assembly);
			Dependency.Container.RegisterType<IUnitOfWork, UnitOfWork<CNCLibContext>>();

			var config = new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<CNCLib.Repository.Contracts.Entities.Machine, CNCLib.Logic.Contracts.DTO.Machine>();
				cfg.CreateMap<CNCLib.Repository.Contracts.Entities.MachineInitCommand, CNCLib.Logic.Contracts.DTO.MachineInitCommand>();
				cfg.CreateMap<CNCLib.Repository.Contracts.Entities.MachineCommand, CNCLib.Logic.Contracts.DTO.MachineCommand>();

				cfg.CreateMap<CNCLib.Logic.Contracts.DTO.Machine, CNCLib.Repository.Contracts.Entities.Machine>();
				cfg.CreateMap<CNCLib.Logic.Contracts.DTO.MachineInitCommand, CNCLib.Repository.Contracts.Entities.MachineInitCommand>();
				cfg.CreateMap<CNCLib.Logic.Contracts.DTO.MachineCommand, CNCLib.Repository.Contracts.Entities.MachineCommand>();
			});

			var mapper = config.CreateMapper();

			Dependency.Container.RegisterInstance<IMapper>(mapper);


			AreaRegistration.RegisterAllAreas();
			GlobalConfiguration.Configure(WebApiConfig.Register);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}
	}
}
