/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) Herbert Aitenbichler

  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

namespace CNCLib.WpfClient.Sql.Start;

using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

using AutoMapper;

using CNCLib.GCode.GUI;
using CNCLib.Logic;
using CNCLib.Logic.Client;
using CNCLib.Repository;
using CNCLib.Repository.Context;
using CNCLib.Repository.SqlServer;
using CNCLib.Service.Logic;
using CNCLib.Shared;

using Framework.Arduino.SerialCommunication;
using Framework.Dependency;
using Framework.Logic;
using Framework.Tools;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NLog;

using ILogger = NLog.ILogger;

public partial class App : Application
{
    private ILogger _logger => LogManager.GetCurrentClassLogger();

    private void AppStartup(object sender, StartupEventArgs e)
    {
        string connectString = SqlServerDatabaseTools.ConnectString;

        GlobalDiagnosticsContext.Set("logDir",           $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/CNCLib.Sql/logs");
        GlobalDiagnosticsContext.Set("connectionString", connectString);

        try
        {
#if DEBUG
            LogManager.ThrowExceptions = true;
#endif
            NLog.Logger logger = LogManager.GetLogger("foo");

            _logger.Info(@"Starting ...");
#if DEBUG
            LogManager.ThrowExceptions = false;
#endif
        }
        catch (SqlException)
        {
            // ignore Sql Exception
        }

        FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

        var userContext = new CNCLibUserContext(CNCLibConst.AdminUser);

        AppService.ServiceCollection = new ServiceCollection();
        AppService.ServiceCollection
            .AddTransient<ILoggerFactory, LoggerFactory>()
            .AddTransient(typeof(ILogger<>), typeof(Logger<>))
            .AddSingleton((ICNCLibUserContext)userContext)
            .AddFrwTools()
            .AddArduinoSerial()
            .AddFrwLogic(
                new MapperConfiguration(
                    cfg =>
                    {
                        cfg.AddProfile<LogicAutoMapperProfile>();
                        cfg.AddProfile<WpfAutoMapperProfile>();
                        cfg.AddProfile<GCodeGUIAutoMapperProfile>();
                    }))
            .AddCNCLibLogic()
            .AddCNCLibLogicClient()
            .AddCNCLibRepository(SqlServerDatabaseTools.OptionBuilder)
            .AddCNCLibServicesLogic()
            .AddWpfClient();

        AppService.BuildServiceProvider();

        // Open Database here

        try
        {
            CNCLibContext.InitializeDatabase(AppService.ServiceProvider);
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