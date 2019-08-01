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
using System.Reflection;
using System.Threading;

using AutoMapper;

using CNCLib.Logic;
using CNCLib.Logic.Client;
using CNCLib.Repository;
using CNCLib.Repository.Context;
using CNCLib.Repository.SqlServer;
using CNCLib.Service.Logic;
using CNCLib.Shared;
using CNCLib.WebAPI;
using CNCLib.WebAPI.Controllers;
using CNCLib.WebAPI.Hubs;

using Framework.Dependency;
using Framework.Mapper;
using Framework.Tools;
using Framework.WebAPI.Filter;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using Newtonsoft.Json.Serialization;

using NLog;

namespace CNCLib.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            string sqlConnectString =
                Microsoft.Azure.Web.DataProtection.Util.IsAzureEnvironment()
                    ? $"Data Source = cnclibdb.database.windows.net; Initial Catalog = CNCLibDb; Persist Security Info = True; User ID = {Xxx}; Password = {Yyy};"
                    : SqlServerDatabaseTools.ConnectString;

            SqlServerDatabaseTools.ConnectString = sqlConnectString;

            GlobalDiagnosticsContext.Set("connectionString", sqlConnectString);
            GlobalDiagnosticsContext.Set("version",          Assembly.GetExecutingAssembly().GetName().Version.ToString());
            GlobalDiagnosticsContext.Set("application",      "CNCLib.WebAPI.Server");
            GlobalDiagnosticsContext.Set("username",         Environment.UserName);
        }

        public        IConfiguration         Configuration { get; }
        public static IServiceProvider       Services      { get; private set; }
        public static IHubContext<CNCLibHub> Hub           => Services.GetService<IHubContext<CNCLibHub>>();

        public void ConfigureServices(IServiceCollection services)
        {
            var controllerAssembly = typeof(CambamController).Assembly;

            services.AddControllers();

            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

            services.AddSignalR(hu => hu.EnableDetailedErrors = true);

            services.AddTransient<UnhandledExceptionFilter>();
            services.AddTransient<ValidateRequestDataFilter>();
            services.AddTransient<MethodCallLogFilter>();
            services.AddMvc(
                    options =>
                    {
                        options.EnableEndpointRouting = false;
                        options.Filters.AddService<ValidateRequestDataFilter>();
                        options.Filters.AddService<UnhandledExceptionFilter>();
                        options.Filters.AddService<MethodCallLogFilter>();
                    })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson(
                    options =>
                        options.SerializerSettings.ContractResolver = new DefaultContractResolver())
                .AddApplicationPart(controllerAssembly);

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/dist"; });

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "CNCLib API", Version = "v1" }); });

            GlobalServiceCollection.Instance = services;
            services
                .AddFrameWorkTools()
                .AddRepository(SqlServerDatabaseTools.OptionBuilder)
                .AddLogic()
                .AddLogicClient()
                .AddServiceAsLogic() // used for Logic.Client
                .AddScoped<ICNCLibUserContext, CNCLibUserContext>()
                .AddMapper(new MapperConfiguration(cfg => { cfg.AddProfile<LogicAutoMapperProfile>(); }));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            Services = app.ApplicationServices;

            CNCLibContext.InitializeDatabase2(false, false);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();
            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            void callback(object x)
            {
                Hub.Clients.All.SendAsync("heartbeat");
            }

            var timer = new Timer(callback);
            timer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(30));

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "CNCLib API V1"); });

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapHub<CNCLibHub>("/serialSignalR");
                    endpoints.MapDefaultControllerRoute();
                });
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

        public string Xxx => @"Herbert";
        public string Yyy => @"Edith1234";
    }
}