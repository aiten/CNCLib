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
using CNCLib.Logic.Abstraction;
using CNCLib.Logic.Client;
using CNCLib.Logic.Manager;
using CNCLib.Repository;
using CNCLib.Repository.Context;
using CNCLib.Repository.SqlServer;
using CNCLib.Service.Logic;
using CNCLib.Shared;
using CNCLib.WebAPI;
using CNCLib.WebAPI.Controllers;
using CNCLib.WebAPI.Filter;
using CNCLib.WebAPI.Hubs;

using Framework.Dependency;
using Framework.Localization;
using Framework.Logic;
using Framework.Logic.Abstraction;
using Framework.Schedule;
using Framework.Schedule.Abstraction;
using Framework.Tools;
using Framework.Tools.Password;
using Framework.WebAPI.Filter;

using Microsoft.AspNetCore.Authentication;
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

using Swashbuckle.AspNetCore.Filters;

namespace CNCLib.Server
{
    public class Startup
    {
        private const string CorsAllowAllName     = "AllowAll";
        private const string AuthenticationScheme = "BasicAuthentication";

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

        public        IConfiguration                           Configuration { get; }
        public static IServiceProvider                         Services      { get; private set; }
        public static IHubContext<CNCLibHub, ICNCLibHubClient> Hub           => Services.GetService<IHubContext<CNCLibHub, ICNCLibHubClient>>();

        public void ConfigureServices(IServiceCollection services)
        {
            var controllerAssembly = typeof(CambamController).Assembly;

            var localizationCollector = new LocalizationCollector();
            localizationCollector.Resources.Add(Framework.Logic.ErrorMessages.ResourceManager);
            localizationCollector.Resources.Add(Framework.Repository.ErrorMessages.ResourceManager);

            services.AddSingleton<LocalizationCollector>(localizationCollector);

            services.AddControllers();

            services.AddCors(options => options.AddPolicy(CorsAllowAllName, options => options.SetIsOriginAllowed(x => _ = true).AllowAnyMethod().AllowAnyHeader().AllowCredentials()));

            services.AddSignalR(hu => hu.EnableDetailedErrors = true);

            services.AddTransient<SetUserContextFilter>();
            services.AddTransient<UnhandledExceptionFilter>();
            services.AddTransient<MethodCallLogFilter>();
            services.AddMvc(
                    options =>
                    {
                        options.EnableEndpointRouting = false;
                        options.Filters.AddService<UnhandledExceptionFilter>();
                        options.Filters.AddService<MethodCallLogFilter>();
                        options.Filters.AddService<SetUserContextFilter>();
                    })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson(
                    options =>
                        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver())
                .AddApplicationPart(controllerAssembly);

            services.AddAuthentication(AuthenticationScheme)
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(AuthenticationScheme, null);

            services.AddScoped<IAuthenticationManager, UserManager>();
            services.AddTransient<IOneWayPasswordProvider, Pbkdf2PasswordProvider>();

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
                .AddFrameWorkTools()
                .AddJobScheduler()
                .AddRepository(SqlServerDatabaseTools.OptionBuilder)
                .AddLogic()
                .AddLogicClient()
                .AddServiceAsLogic() // used for Logic.Client
                .AddScoped<ICNCLibUserContext, CNCLibUserContext>()
                .AddMapper(new MapperConfiguration(cfg => { cfg.AddProfile<LogicAutoMapperProfile>(); }));

            AppService.ServiceCollection = services;
            AppService.BuildServiceProvider();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            Services = app.ApplicationServices;

            CNCLibContext.InitializeDatabase2();

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
                Hub.Clients.All.HeartBeat();
            }

            var timer = new Timer(callback);
            timer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(30));

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "CNCLib API V1"); });

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapHub<CNCLibHub>("/cncLibSignalR");
                    endpoints.MapDefaultControllerRoute();
                });

            app.UseSpa(
                spa =>
                {
                    spa.Options.SourcePath = "ClientApp";

                    if (env.IsDevelopment())
                    {
                        spa.UseAngularCliServer(npmScript: "start");
                    }
                });


            var scheduler = Services.GetRequiredService<IJobScheduler>();
            scheduler
                .Periodic<ICleanupJob>(TimeSpan.FromMinutes(1), "Cleanup 1")
                .Then<ICleanupJob>("Cleanup 2")
                .Then<ICleanupJob>("Cleanup 3");
            scheduler.Daily<IDailyJob>(TimeSpan.Parse("02:00"), "Hallo from daily");
        }

        public string Xxx => @"Herbert";
        public string Yyy => @"Edith1234";
    }
}