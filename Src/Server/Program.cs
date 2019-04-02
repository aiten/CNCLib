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