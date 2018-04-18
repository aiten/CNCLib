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

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace CNCLib.Serial.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (RunsAsService())
            {
                var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                Environment.CurrentDirectory = dir;

                ServiceBase.Run(new ServiceBase[] { new CNCLibServerService() });
            }
            else
            {
                BuildWebHost(args).Run();
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
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return CheckForConsoleWindow();
            }

            return false;   // never can be a windows service
        }

        private sealed class CNCLibServerService : ServiceBase
        {
            private IWebHost _webHost;
            protected override void OnStart(string[] args)
            {
                try
                {
//                  string[] imagePathArgs = Environment.GetCommandLineArgs();
                    _webHost = BuildWebHost(args);
                    _webHost.Start();
                }
                catch (Exception e)
                {
                    File.AppendAllText(Path.Combine(Path.GetTempPath(),@"CNCLibError.txt"), "OnStartError: " + e.Message + Environment.NewLine);
                    throw;
                }
            }

            protected override void OnStop()
            {
                _webHost.Dispose();
            }
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hosting.json", optional: true)
                .AddCommandLine(args)
                .Build();
            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseConfiguration(config)
                .UseStartup<Startup>()
                .Build();
        }
    }
}
