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

using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

using AutoMapper;

using CNCLib.GCode.GUI;
using CNCLib.Logic.Client;
using CNCLib.Service.Abstraction;
using CNCLib.Service.WebAPI;
using CNCLib.Shared;

using Framework.Arduino.SerialCommunication;
using Framework.Dependency;
using Framework.Logging;
using Framework.Mapper;
using Framework.Tools;

using Microsoft.Extensions.DependencyInjection;

using NLog;

namespace CNCLib.WpfClient.WebAPI.Start
{
    public partial class App : Application
    {
        private ILogger _logger => LogManager.GetCurrentClassLogger();

        private void AppStartup(object sender, StartupEventArgs e)
        {
            GlobalDiagnosticsContext.Set("logDir", $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/CNCLib.Web/logs");

            _logger.Info(@"Starting ...");
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            ICNCLibUserContext userContext = new CNCLibUserContext();

            GlobalServiceCollection.Instance = new ServiceCollection();
            GlobalServiceCollection.Instance

                .AddFrameWorkTools()
                .AddFrameworkLogging()
                .AddLogicClient()
                .AddSerialCommunication()
                .AddServiceAsWebAPI()
                .AddCNCLibWpf()

                .AddMapper(
                    new MapperConfiguration(
                        cfg =>
                        {
                            cfg.AddProfile<WpfAutoMapperProfile>();
                            cfg.AddProfile<GCodeGUIAutoMapperProfile>();
                        }))

                .AddSingleton(userContext);

            Framework.Service.WebAPI.HttpClientFactory.Instance.RegisterCreateHttpClient(
                @"http://cnclibwebapi.azurewebsites.net",
                s =>
                {
                    var client = new HttpClient() { BaseAddress = new System.Uri(s) };
                    Framework.Service.WebAPI.HttpClientFactory.DefaultConfigureHttpClient(client);
                    return client;

                });

            // Open WebAPI Connection
            //
            bool ok = Task.Run(
                async () =>
                {
                    try
                    {
                        using (var controller = GlobalServiceCollection.Instance.Resolve<IMachineService>())
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