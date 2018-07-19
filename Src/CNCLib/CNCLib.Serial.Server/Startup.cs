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

using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;
using CNCLib.Serial.Server.Hubs;
using Framework.Tools.Dependency;
using Framework.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.AspNetCore.SpaServices.Webpack;
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

        public IConfiguration Configuration { get; }
        public static IServiceProvider Services { get; private set; }
        public static IHubContext<CNCLibHub> Hub => Services.GetService<IHubContext<CNCLibHub>>();

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy("AllowAll", p => 
                p.AllowAnyOrigin()
                .AllowCredentials()
                .AllowAnyMethod()
                .AllowAnyHeader()));

            services.AddSignalR((HubOptions hu) => hu.EnableDetailedErrors = true);

            services.AddMvc().
				SetCompatibilityVersion(CompatibilityVersion.Version_2_1).
                AddJsonOptions(options =>
                options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "CNCLib API", Version = "v1" });
            });

            Dependency.Initialize(new AspNetDependencyProvider(services));
            Dependency.Container.RegisterTypesIncludingInternalsScoped(
                typeof(Framework.Arduino.SerialCommunication.Serial).Assembly);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Dependency.Container.RegisterTypeScoped<Framework.Arduino.SerialCommunication.ISerialPort,
                    Framework.Arduino.SerialCommunication.SerialPortLib>();
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            Services = app.ApplicationServices;
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

            app.UseSignalR(router =>
            {
                router.MapHub<CNCLibHub>("/serialSignalR");
            });

            void callback(object x)
            {
                Hub.Clients.All.SendAsync("heartbeat");
            }
            var timer = new Timer(callback);
            timer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(30));

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CNCLib API V1");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{instance}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
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
