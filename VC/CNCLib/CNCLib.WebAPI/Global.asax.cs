////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

  CNCLib is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  CNCLib is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  http://www.gnu.org/licenses/
*/

using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AutoMapper;
using CNCLib.Logic;
using CNCLib.Repository.Context;
using CNCLib.WebAPI.Controllers;
using Framework.EF;
using Framework.Tools.Dependency;
using Framework.Tools.Pattern;
using Framework.Web;

namespace CNCLib.WebAPI
{
	public class WebApiApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			Dependency.Initialize(new LiveDependencyProvider());
			Dependency.Container.RegisterTypesIncludingInternals(typeof(CNCLib.Repository.MachineRepository).Assembly, typeof(CNCLib.Logic.MachineController).Assembly);
			Dependency.Container.RegisterType<IUnitOfWork, UnitOfWork<CNCLibContext>>();

			Dependency.Container.RegisterType<IRest< CNCLib.Logic.Contracts.DTO.Machine>, MachineRest>();
			Dependency.Container.RegisterType<IRest<CNCLib.Logic.Contracts.DTO.LoadOptions>, LoadInfoRest>();

			var config = new MapperConfiguration(cfg =>
			{
				cfg.AddProfile<LogicAutoMapperProfile>();
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
