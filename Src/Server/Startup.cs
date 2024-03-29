﻿/*
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

namespace CNCLib.Server
{
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
    using CNCLib.Repository.SqLite;
    using CNCLib.Service.Logic;
    using CNCLib.Shared;
    using CNCLib.WebAPI;
    using CNCLib.WebAPI.Controllers;
    using CNCLib.WebAPI.Filter;
    using CNCLib.WebAPI.Hubs;

    using Framework.Dependency;
    using Framework.Localization;
    using Framework.Localization.Abstraction;
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
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.AspNetCore.SpaServices.AngularCli;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;

    using Newtonsoft.Json.Serialization;

    using NLog;

    using Swashbuckle.AspNetCore.Filters;

    public class Startup
    {
        private const string CorsAllowAllName     = "AllowAll";
        private const string AuthenticationScheme = "BasicAuthentication";

        private static readonly TimeSpan _flushStatisticsTime = TimeSpan.FromMinutes(1);

        private IJobExecutor? _flushCallStatisticsJob;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            string connectString = SqliteDatabaseTools.SetEnvironment(Microsoft.Azure.Web.DataProtection.Util.IsAzureEnvironment());

            GlobalDiagnosticsContext.Set("connectionString", connectString);
            GlobalDiagnosticsContext.Set("version",          Assembly.GetExecutingAssembly().GetName().Version?.ToString());
            GlobalDiagnosticsContext.Set("application",      "CNCLib.WebAPI.Server");
            GlobalDiagnosticsContext.Set("username",         Environment.UserName);
        }

        public        IConfiguration                            Configuration { get; }
        public static IServiceProvider                          Services      { get; private set; } = default!;
        public static IHubContext<CNCLibHub, ICNCLibHubClient>? Hub           => Services.GetService<IHubContext<CNCLibHub, ICNCLibHubClient>>();

        public void ConfigureServices(IServiceCollection services)
        {
            var localizationCollector = new LocalizationCollector();

            var controllerAssembly = typeof(CambamController).Assembly;

            services.AddSingleton<ILocalizationCollector>(localizationCollector);

            services.AddControllers();

            services.AddCors(options => options.AddPolicy(CorsAllowAllName, config => config.SetIsOriginAllowed(x => _ = true).AllowAnyMethod().AllowAnyHeader().AllowCredentials()));

            services.AddSignalR(hu => hu.EnableDetailedErrors = true);

            services.AddTransient<GCodeLoadHelper>();
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
                .AddNewtonsoftJson(
                    options =>
                        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver())
                .AddApplicationPart(controllerAssembly);

            services.AddAuthentication(AuthenticationScheme)
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(AuthenticationScheme, null);

            services.AddAuthorization(options => { options.AddPolicy(Policies.IsAdmin, policy => policy.RequireClaim(CNCLibClaimTypes.IsAdmin)); });

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

            services.AddCNCLibLogic()
                .AddCNCLibLogicClient()
                .AddCNCLibRepository(SqliteDatabaseTools.OptionBuilder)
                .AddCNCLibServicesLogic()
                .AddFrwTools()
                .AddFrwSchedule()
                .AddFrwLogic(new MapperConfiguration(cfg => { cfg.AddProfile<LogicAutoMapperProfile>(); }));


            services.AddScoped<ICNCLibUserContext, CNCLibUserContext>();
        }

        private void OnShutdown()
        {
            _flushCallStatisticsJob?.Execute().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            var applicationLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            applicationLifetime.ApplicationStopping.Register(OnShutdown);

            AppService.ServiceProvider = app.ApplicationServices;

            Services = app.ApplicationServices;

            CNCLibContext.InitializeDatabase(Services);

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

            void callback(object? x)
            {
                Hub!.Clients.All.HeartBeat();
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

            void SetJobExecutor(IJobExecutor executor, object param)
            {
                // executor.ParamContainer = typeof(JobParamContainer); => this is default
                //executor.JobName  = xxx => default is Classname
                executor.Param = param;
            }

            var scheduler = Services.GetRequiredService<IJobScheduler>();

            scheduler.Daily<IDailyJob>(TimeSpan.Parse("02:00"), executor => SetJobExecutor(executor, "Hallo from daily"));

            _flushCallStatisticsJob = scheduler.Periodic<IFlushCallStatisticJob>(_flushStatisticsTime);

            scheduler.Start();
        }
    }
}