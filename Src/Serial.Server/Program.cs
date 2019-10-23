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

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using NLog.Web;

using System;
using System.IO;
using System.Runtime.InteropServices;

using Framework.WebAPI.Host;

using NLog;

namespace CNCLib.Serial.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
            LogManager.ThrowExceptions = true;
#endif
            string localAppData;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                localAppData = "/var/log";
            }
            else
            {
                localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                if (!Directory.Exists(localAppData) || ProgramUtilities.RunsAsService())
                {
                    // service user
                    localAppData = Environment.GetEnvironmentVariable("ProgramData");
                }
            }

            GlobalDiagnosticsContext.Set("logDir", $"{localAppData}/CNCLib.Serial.Server/logs");
            var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
            try
            {
                logger.Info("Starting (Main)");
                ProgramUtilities.StartWebService(args, BuildWebHost);
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }
        }

        private static IWebHostBuilder BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(
                    (hostingContext, config) => { config.AddJsonFile("hosting.json", optional: true); })
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog();
        }
    }
}