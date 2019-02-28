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
using System.Runtime.InteropServices;
using System.Threading;

using CNCLib.Serial.WebAPI.Controllers;
using CNCLib.Serial.WebAPI.Hubs;
using CNCLib.Serial.WebAPI.SerialPort;

using Framework.Arduino.Linux.SerialCommunication;
using Framework.Arduino.SerialCommunication.Abstraction;
using Framework.Dependency;
using Framework.Tools;
using Framework.Logging;
using Framework.WebAPI;
using Framework.WebAPI.Filter;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json.Serialization;

using Swashbuckle.AspNetCore.Swagger;

namespace CNCLib.Serial.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public        IConfiguration         Configuration { get; }
        public static IServiceProvider       Services      { get; private set; }
        public static IHubContext<CNCLibHub> Hub           => Services.GetService<IHubContext<CNCLibHub>>();

        public void ConfigureServices(IServiceCollection services)
        {
            var controllerAssembly = typeof(HomeController).Assembly;

            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowCredentials().AllowAnyMethod().AllowAnyHeader()));

            services.AddSignalR(hu => hu.EnableDetailedErrors = true);

            services.AddTransient<UnhandledExceptionFilter>();
            services.AddTransient<ValidateRequestDataFilter>();
            services.AddMvc(
                options =>
                {
                    options.Filters.AddService<ValidateRequestDataFilter>();
                    options.Filters.AddService<UnhandledExceptionFilter>();
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver())
                .AddApplicationPart(controllerAssembly);


            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/dist"; });

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new Info { Title = "CNCLib API", Version = "v1" }); });

            Dependency.Initialize(new MsDependencyProvider(services));

            Dependency.Container.RegisterFrameWorkTools();
            Dependency.Container.RegisterFrameWorkLogging();

            Dependency.Container.RegisterTypesIncludingInternalsScoped(typeof(Framework.Arduino.SerialCommunication.Serial).Assembly);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Dependency.Container.RegisterTypeScoped<ISerialPort, SerialPortLib>();
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            Services = app.ApplicationServices;

            SerialPortWrapper.OnCreateHub = () => Hub;
            //Services = serviceProvider;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseCors("AllowAll");

            app.UseSignalR(router => { router.MapHub<CNCLibHub>("/serialSignalR"); });

            void callback(object x)
            {
                Hub.Clients.All.SendAsync("heartbeat");
            }

            var timer = new Timer(callback);
            timer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(30));

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "CNCLib API V1"); });

            app.UseMvc(routes => { routes.MapRoute(name: "default", template: "{instance}/{action=Index}/{id?}"); });

            app.UseSpa(
                spa =>
                {
                    // To learn more about options for serving an Angular SPA from ASP.NET Core,
                    // see https://go.microsoft.com/fwlink/?linkid=864501

                    spa.Options.SourcePath = "ClientApp";

                    if (env.IsDevelopment())
                    {
                        spa.UseAngularCliServer(npmScript: "start");
                    }
                });
        }
    }
}