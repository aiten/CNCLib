////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2018 Herbert Aitenbichler

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
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using AutoMapper;
using CNCLib.GCode.GUI;
using CNCLib.ServiceProxy;
using Framework.Tools.Dependency;
using Framework.Tools.Pattern;

namespace CNCLib.Wpf.WebAPI.Start
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
	{
		private void AppStartup(object sender, StartupEventArgs e)
		{
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(
                XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            Dependency.Initialize(new LiveDependencyProvider());
			Dependency.Container.RegisterTypesIncludingInternalsScoped(
                typeof(Framework.Arduino.SerialCommunication.Serial).Assembly,
                //				typeof(CNCLib.ServiceProxy.Logic.MachineService).Assembly,
                typeof(ServiceProxy.WebAPI.MachineService).Assembly,
				typeof(Logic.Client.DynItemController).Assembly);
            //			Dependency.Container.RegisterType<IUnitOfWork, UnitOfWork<CNCLibContext>>();

		    Dependency.Container.RegisterTypeScoped<IFactory<IMachineService>, FactoryResolve<IMachineService>>();
		    Dependency.Container.RegisterTypeScoped<IFactory<ILoadOptionsService>, FactoryResolve<ILoadOptionsService>>();

            Dependency.Container.RegisterTypesByNameScoped(
                n => n.EndsWith("ViewModel"),
                typeof(ViewModels.MachineViewModel).Assembly,
                typeof(GCode.GUI.ViewModels.LoadOptionViewModel).Assembly);

            var config = new MapperConfiguration(cfg =>
				{
//					cfg.AddProfile<LogicAutoMapperProfile>();
					cfg.AddProfile<WpfAutoMapperProfile>();
                    cfg.AddProfile<GCodeGUIAutoMapperProfile>();

                });
			config.AssertConfigurationIsValid();

			IMapper mapper = config.CreateMapper();
			Dependency.Container.RegisterInstance(mapper);

			// Open WebAPI Connection
			//
			bool ok = Task.Run(async () =>
			{
				try
				{
					using (var controller = Dependency.Resolve<IMachineService>())
					{
						var m = await controller.Get(1000000);
/*
						if (m == -1)
						{
							throw new ArgumentException("cannot connect to service");
						}
*/
					}
					return true;
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Cannot connect to WebAPI: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					Current.Shutdown();
					return false;
				}
			}).ConfigureAwait(true).GetAwaiter().GetResult();
		}
	}
}
