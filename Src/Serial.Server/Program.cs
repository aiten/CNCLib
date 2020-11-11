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

namespace CNCLib.Serial.Server
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Hosting.WindowsServices;

    using NLog.Web;

    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    using Framework.WebAPI.Host;

    using NLog;

    public class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
            LogManager.ThrowExceptions = true;
#endif
            string logDir = string.Empty;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                logDir = "/var/log/CNCLib.Serial.Server";
            }
            else
            {
                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                if (!Directory.Exists(localAppData) || WindowsServiceHelpers.IsWindowsService())
                {
                    // service user
                    localAppData = Environment.GetEnvironmentVariable("ProgramData");
                }

                logDir = $"{localAppData}/CNCLib.Serial.Server/logs";
            }

            GlobalDiagnosticsContext.Set("logDir", logDir);
            var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
            try
            {
                logger.Info("Starting (Main)");
                ProgramUtilities.StartWebService(args, BuildHost);
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }
        }

        private static IHostBuilder BuildHost(string[] args)
        {
            var hostConfig = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hosting.json", optional: true)
                .AddCommandLine(args)
                .Build();

            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseConfiguration(hostConfig)
                        .UseStartup<Startup>()
                        .ConfigureLogging(logging =>
                        {
                            logging.ClearProviders();
                            logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                        })
                        .UseNLog();
                });
        }
    }
}