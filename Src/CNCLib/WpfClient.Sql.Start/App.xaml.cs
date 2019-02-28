/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2019 Herbert Aitenbichler

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
using System.Data.SqlClient;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

using AutoMapper;

using CNCLib.GCode.GUI;
using CNCLib.Logic;
using CNCLib.Logic.Client;
using CNCLib.Repository;
using CNCLib.Repository.SqlServer;
using CNCLib.Service.Logic;
using CNCLib.Shared;

using Framework.Arduino.SerialCommunication;
using Framework.Dependency;
using Framework.Mapper;
using Framework.Tools;

using NLog;

namespace CNCLib.WpfClient.Sql.Start
{
    public partial class App : Application
    {
        private ILogger _logger => LogManager.GetCurrentClassLogger();

        private void AppStartup(object sender, StartupEventArgs e)
        {
            GlobalDiagnosticsContext.Set("logDir",           $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/CNCLib.Sql/logs");
            GlobalDiagnosticsContext.Set("connectionString", MigrationCNCLibContext.ConnectString);

            try
            {
#if DEBUG
                LogManager.ThrowExceptions = true;
#endif
                Logger logger = LogManager.GetLogger("foo");

                _logger.Info(@"Starting ...");
#if DEBUG
                LogManager.ThrowExceptions = false;
#endif
            }
            catch (SqlException exception)
            {
                // ignore Sql Exception
            }

            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            Dependency.Initialize(new LiveDependencyProvider());

            Dependency.Container.RegisterFrameWorkTools();
            Framework.Logging.LiveDependencyRegisterExtensions.RegisterFrameWorkLogging(Dependency.Container);
            Dependency.Container.RegisterRepository();
            Dependency.Container.RegisterLogic();
            Dependency.Container.RegisterLogicClient();
            Dependency.Container.RegisterSerialCommunication();
            Dependency.Container.RegisterServiceAsLogic();
            Dependency.Container.RegisterCNCLibWpf();
            Dependency.Container.RegisterMapper(
                new MapperConfiguration(
                    cfg =>
                    {
                        cfg.AddProfile<LogicAutoMapperProfile>();
                        cfg.AddProfile<WpfAutoMapperProfile>();
                        cfg.AddProfile<GCodeGUIAutoMapperProfile>();
                    }));

            var userContext = new CNCLibUserContext();
            Dependency.Container.RegisterInstance((ICNCLibUserContext)userContext);

            //	        string sqlConnectString = @"Data Source = (LocalDB)\MSSQLLocalDB; Initial Catalog = CNCLib; Integrated Security = True";
            string sqlConnectString = null;

            // Open Database here

            try
            {
                Repository.SqlServer.MigrationCNCLibContext.InitializeDatabase(sqlConnectString, false, false);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);

                MessageBox.Show("Cannot connect to database" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
            }

            var task = Task.Run(async () => await userContext.InitUserContext());
            while (!task.IsCompleted)
            {
                Task.Yield();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            LogManager.Shutdown();
        }
    }
}