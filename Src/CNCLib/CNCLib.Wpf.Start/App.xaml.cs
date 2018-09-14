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
using CNCLib.Logic.Manager;
using CNCLib.Repository.Context;
using CNCLib.Service.Contracts;
using CNCLib.Service.Logic;
using CNCLib.Shared;
using Framework.Contracts.Repository;
using Framework.Contracts.Shared;
using Framework.Repository;
using Framework.Tools;
using Framework.Tools.Dependency;
using Framework.Tools.Pattern;
using NLog;

namespace CNCLib.Wpf.Start
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ILogger _logger => LogManager.GetCurrentClassLogger();

        private void AppStartup(object sender, StartupEventArgs e)
        {
            _logger.Info(@"Starting ...");

            string userprofilepath = Environment.GetEnvironmentVariable(@"USERPROFILE");
            AppDomain.CurrentDomain.SetData("DataDirectory", userprofilepath);

            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            Dependency.Initialize(new LiveDependencyProvider());

            Dependency.Container.RegisterType<ICurrentDateTime, CurrentDateTime>();
            Dependency.Container.RegisterTypeScoped<CNCLibContext, CNCLibContext>();

            Dependency.Container.RegisterTypesIncludingInternals(typeof(Framework.Arduino.SerialCommunication.Serial).Assembly, typeof(MachineService).Assembly,
//				typeof(CNCLib.ServiceProxy.WebAPI.MachineService).Assembly,
                                                                 typeof(Repository.MachineRepository).Assembly, typeof(Logic.Client.DynItemController).Assembly, typeof(MachineManager).Assembly);

            Dependency.Container.RegisterTypeScoped<IUnitOfWork, UnitOfWork<CNCLibContext>>();

            Dependency.Container.RegisterType<IFactory<IMachineService>, FactoryResolve<IMachineService>>();
            Dependency.Container.RegisterType<IFactory<ILoadOptionsService>, FactoryResolve<ILoadOptionsService>>();

            Dependency.Container.RegisterTypesByName(n => n.EndsWith("ViewModel"), typeof(ViewModels.MachineViewModel).Assembly, typeof(GCode.GUI.ViewModels.LoadOptionViewModel).Assembly);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<LogicAutoMapperProfile>();
                cfg.AddProfile<WpfAutoMapperProfile>();
                cfg.AddProfile<GCodeGUIAutoMapperProfile>();
            });
            config.AssertConfigurationIsValid();

            IMapper mapper = config.CreateMapper();
            Dependency.Container.RegisterInstance(mapper);

            ICNCLibUserContext userContext = new CNCLibUserContext();
            Dependency.Container.RegisterInstance(userContext);

            // Open Database here

            string dbfile = userprofilepath + @"\CNCLib.db";
            try
            {
                Repository.SqLite.MigrationCNCLibContext.InitializeDatabase(dbfile, false, false);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);

                MessageBox.Show($"Cannot create/connect database in {dbfile} \n\r" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
            }
        }
    }
}