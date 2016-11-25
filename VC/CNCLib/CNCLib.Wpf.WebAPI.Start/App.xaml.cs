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

using System;
using System.Windows;
using CNCLib.Logic.Contracts;
using Framework.Tools.Dependency;
using Framework.Tools.Pattern;
using AutoMapper;
using CNCLib.Logic;
using CNCLib.ServiceProxy;

namespace CNCLib.Wpf.WebAPI.Start
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private void AppStartup(object sender, StartupEventArgs e)
		{
            Dependency.Initialize(new LiveDependencyProvider());
			Dependency.Container.RegisterTypesIncludingInternals(
				//				typeof(CNCLib.ServiceProxy.Logic.MachineService).Assembly,
				typeof(CNCLib.ServiceProxy.WebAPI.MachineService).Assembly,
				typeof(CNCLib.Logic.Client.DynItemController).Assembly);
	//			Dependency.Container.RegisterType<IUnitOfWork, UnitOfWork<CNCLibContext>>();

			var config = new MapperConfiguration(cfg =>
				{
//					cfg.AddProfile<LogicAutoMapperProfile>();
					cfg.AddProfile<WpfAutoMapperProfile>();
				});
			config.AssertConfigurationIsValid();

			var mapper = config.CreateMapper();

			Dependency.Container.RegisterInstance<IMapper>(mapper);


			using (var controller = Dependency.Resolve<IMachineService>())
			{
				var m = controller.Get(-1);
			}
		}
	}
}
