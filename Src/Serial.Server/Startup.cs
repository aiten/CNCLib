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
using CNCLib.Serial.WebAPI.Manager;
using CNCLib.Serial.WebAPI.SerialPort;

using Framework.Arduino.SerialCommunication;
using Framework.Arduino.SerialCommunication.Abstraction;
using Framework.Dependency;
using Framework.Logic.Abstraction;
using Framework.Tools;
using Framework.WebAPI.Filter;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using Newtonsoft.Json.Serialization;

using NLog;

using Swashbuckle.AspNetCore.Filters;

namespace CNCLib.Serial.Server
{
    public class Startup
    {
        private const string CorsAllowAllName = "AllowAll";
        private const string AuthenticationScheme = "BasicAuthentication";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public        IConfiguration         Configuration { get; }
        public static IServiceProvider       Services      { get; private set; }
        public static IHubContext<CNCLibHub> Hub           => Services.GetService<IHubContext<CNCLibHub>>();

        public void ConfigureServices(IServiceCollection services)
        {
            var controllerAssembly = typeof(InfoController).Assembly;

            //services.AddControllers();

            services.AddCors(options => options.AddPolicy(CorsAllowAllName, options => options.SetIsOriginAllowed(x => _ = true).AllowAnyMethod().AllowAnyHeader().AllowCredentials()));

            services.AddSignalR(hu => hu.EnableDetailedErrors = true);

            services.AddTransient<UnhandledExceptionFilter>();
            services.AddTransient<MethodCallLogFilter>();
            services.AddMvc(
                    options =>
                    {
                        options.EnableEndpointRouting = false;
                        options.Filters.AddService<UnhandledExceptionFilter>();
                        options.Filters.AddService<MethodCallLogFilter>();
                    })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson(
                    options =>
                        options.SerializerSettings.ContractResolver = new DefaultContractResolver())
                .AddApplicationPart(controllerAssembly);

            services.AddAuthentication(AuthenticationScheme)
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(AuthenticationScheme, null);

            services.AddScoped<IAuthenticationManager, UserManager>();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/dist"; });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CNCLib API", Version = "v1" });
                c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
                {
                    Name        = "Authorization",
                    Type        = SecuritySchemeType.Http,
                    Scheme      = "basic",
                    In          = ParameterLocation.Header,
                    Description = "Basic Authorization header using the Bearer scheme."
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id   = "basic"
                            }
                        },
                        new string[] { }
                    }
                });
                c.OperationFilter<SecurityRequirementsOperationFilter>(true, "basic");
            });


            services
                .AddSerialCommunication()
                .AddFrameWorkTools();

            AppService.ServiceCollection = services;
            AppService.BuildServiceProvider();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            // app.UsePathBase("/Serial.Server");

            Services = app.ApplicationServices;

            SerialPortWrapper.OnCreateHub = () => Hub;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();
            app.UseHttpsRedirection();

            app.UseCors(CorsAllowAllName);

            app.UseAuthentication();
            app.UseAuthorization();

            void callback(object x)
            {
                Hub.Clients.All.SendAsync("heartbeat");
            }

            var timer = new Timer(callback);
            timer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(30));

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "CNCLib API V1"); });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<CNCLibHub>("/serialSignalR");
                endpoints.MapDefaultControllerRoute();
            });


            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}