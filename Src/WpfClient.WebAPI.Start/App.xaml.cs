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

namespace CNCLib.WpfClient.WebAPI.Start
{
    using System;
    using System.Globalization;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Markup;

    using AutoMapper;

    using CNCLib.GCode.GUI;
    using CNCLib.Service.Abstraction;
    using CNCLib.Shared;

    using Framework.Dependency;
    using Framework.Localization;
    using Framework.Service.WebAPI;
    using Framework.Startup;
    using Framework.Tools;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using NLog;

    using ILogger = NLog.ILogger;

    public partial class App : Application
    {
        private ILogger _logger => LogManager.GetCurrentClassLogger();

        private void AppStartup(object sender, StartupEventArgs e)
        {
            var                userContextRW = new CNCLibUserContext();
            ICNCLibUserContext userContext   = userContextRW;

            var localizationCollector = new LocalizationCollector();
            var moduleInit            = new InitializationManager();

            moduleInit.Add(new Framework.Tools.ModuleInitializer());
            moduleInit.Add(new Framework.Arduino.SerialCommunication.ModuleInitializer());
            moduleInit.Add(new Framework.Logic.ModuleInitializer()
            {
                MapperConfiguration =
                    new MapperConfiguration(
                        cfg =>
                        {
                            cfg.AddProfile<WpfAutoMapperProfile>();
                            cfg.AddProfile<GCodeGUIAutoMapperProfile>();
                        })
            });
            moduleInit.Add(new CNCLib.Logic.Client.ModuleInitializer());
            moduleInit.Add(new CNCLib.WpfClient.ModuleInitializer());
            moduleInit.Add(new CNCLib.Service.WebAPI.ModuleInitializer()
            {
                ConfigureHttpClient = httpClient =>
                {
                    HttpClientHelper.PrepareHttpClient(httpClient, @"https://cnclib.azurewebsites.net");
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue(
                            "Basic", Base64Helper.StringToBase64($"{userContextRW.UserName}:{userContextRW.Password}"));
                }
            });

            GlobalDiagnosticsContext.Set("logDir", $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/CNCLib.Web/logs");

            _logger.Info(@"Starting ...");
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            AppService.ServiceCollection = new ServiceCollection();
            AppService.ServiceCollection
                .AddTransient<ILoggerFactory, LoggerFactory>()
                .AddTransient(typeof(ILogger<>), typeof(Logger<>))
                .AddSingleton(userContext);

            moduleInit.Initialize(AppService.ServiceCollection, localizationCollector);

            AppService.BuildServiceProvider();

            // Open WebAPI Connection
            //
            bool ok = Task.Run(
                async () =>
                {
                    try
                    {
                        await userContextRW.InitUserContext(userContextRW.UserName);

                        using (var scope = AppService.ServiceProvider.CreateScope())
                        {
                            var controller = scope.ServiceProvider.GetRequiredService<IMachineService>();
                            var m          = await controller.Get(1000000);

                            return true;
                        }
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