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
using System.Windows;
using System.Windows.Markup;
using AutoMapper;
using CNCLib.GCode.GUI;
using CNCLib.Logic;
using CNCLib.Repository.Context;
using Framework.EF;
using Framework.Tools.Dependency;
using Framework.Tools.Pattern;

namespace CNCLib.Wpf.Sql.Start
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

		    Dependency.Container.RegisterTypeScoped<CNCLibContext, CNCLibContext>();

            Dependency.Container.RegisterTypesIncludingInternalsScoped(
                typeof(Framework.Arduino.SerialCommunication.Serial).Assembly,
                typeof(ServiceProxy.Logic.MachineService).Assembly,
//				typeof(CNCLib.ServiceProxy.WebAPI.MachineService).Assembly,
				typeof(Repository.MachineRepository).Assembly,
				typeof(Logic.Client.DynItemController).Assembly,
				typeof(MachineController).Assembly);
			Dependency.Container.RegisterTypeScoped<IUnitOfWork, UnitOfWork<CNCLibContext>>();

            Dependency.Container.RegisterTypesByNameScoped(
                n => n.EndsWith("ViewModel"),
                typeof(ViewModels.MachineViewModel).Assembly,
                typeof(GCode.GUI.ViewModels.LoadOptionViewModel).Assembly);

            var config = new MapperConfiguration(cfg =>
				{
					cfg.AddProfile<LogicAutoMapperProfile>();
					cfg.AddProfile<WpfAutoMapperProfile>();
                    cfg.AddProfile<GCodeGUIAutoMapperProfile>();
                });
			config.AssertConfigurationIsValid();

			IMapper mapper = config.CreateMapper();
			Dependency.Container.RegisterInstance(mapper);

//	        string sqlconnectstring = @"Data Source = (LocalDB)\MSSQLLocalDB; Initial Catalog = CNCLib; Integrated Security = True";
		    string sqlconnectstring = null;

            // Open Database here

            try
            {
		        CNCLib.Repository.SqlServer.MigrationCNCLibContext.InitializeDatabase(sqlconnectstring, false);
		    }
			catch (Exception ex)
			{
				MessageBox.Show("Cannot connect to database" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				Current.Shutdown();
			}
		}
	}
}
