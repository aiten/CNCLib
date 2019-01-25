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
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using NLog;
using NLog.Web;

using ILogger = NLog.ILogger;

namespace CNCLib.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            GlobalDiagnosticsContext.Set("logDir", $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/CNCLib.Web/logs");

            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

#if DEBUG
            LogManager.ThrowExceptions = true;
#endif
            try
            {
                StartWebService(args);
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }
        }

        private static void StartWebService(string[] args)
        {
            if (RunsAsService())
            {
                Environment.CurrentDirectory = BaseDirectory;

                ServiceBase.Run(new ServiceBase[] { new CNCLibServerService() });
            }
            else
            {
                BuildWebHost(args).Run();
                LogManager.Shutdown();
            }
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        private static bool CheckForConsoleWindow()
        {
            return GetConsoleWindow() == IntPtr.Zero;
        }

        private static bool RunsAsService()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Microsoft.Azure.Web.DataProtection.Util.IsAzureEnvironment() == false)
            {
                return CheckForConsoleWindow();
            }

            return false; // never can be a windows service
        }

        private sealed class CNCLibServerService : ServiceBase
        {
            private          IWebHost _webHost;
            private readonly ILogger  _logger = LogManager.GetCurrentClassLogger();

            protected override void OnStart(string[] args)
            {
                try
                {
                    _webHost = BuildWebHost(args);
                    _webHost.Start();
                }
                catch (Exception e)
                {
                    _logger.Fatal(e);
                    throw;
                }
            }

            protected override void OnStop()
            {
                LogManager.Shutdown();
                _webHost.Dispose();
            }
        }

        private static string BaseDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private static IWebHost BuildWebHost(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hosting.json", optional: true)
                .AddCommandLine(args).Build();
            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseConfiguration(config)
                .UseStartup<Startup>()
                .ConfigureLogging(logging => { logging.ClearProviders(); })
                .UseNLog()
                .Build();
        }
    }
}