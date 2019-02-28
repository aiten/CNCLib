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

using AutoMapper;

using CNCLib.Logic;
using CNCLib.Logic.Client;
using CNCLib.Repository;
using CNCLib.Repository.SqlServer;
using CNCLib.Shared;
using CNCLib.WebAPI;
using CNCLib.WebAPI.Controllers;

using Framework.Dependency;
using Framework.Logging;
using Framework.Mapper;
using Framework.Tools;
using Framework.WebAPI.Filter;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json.Serialization;

using NLog;

using Swashbuckle.AspNetCore.Swagger;

namespace CNCLib.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var controllerAssembly = typeof(CambamController).Assembly;

            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

            services.AddTransient<UnhandledExceptionFilter>();
            services.AddTransient<ValidateRequestDataFilter>();
            services.AddTransient<MethodCallLogFilter>();
            services.AddMvc(
                options =>
                {
                    options.Filters.AddService<ValidateRequestDataFilter>();
                    options.Filters.AddService<UnhandledExceptionFilter>();
                    options.Filters.AddService<MethodCallLogFilter>();
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver())
                .AddApplicationPart(controllerAssembly);


            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new Info { Title = "CNCLib API", Version = "v1" }); });

            Dependency.Initialize(new MsDependencyProvider(services));

            Dependency.Container.RegisterFrameWorkTools();
            Dependency.Container.RegisterFrameWorkLogging();
            Dependency.Container.RegisterRepository();
            Dependency.Container.RegisterLogic();
            Dependency.Container.RegisterLogicClient();

            Dependency.Container.RegisterTypeScoped<ICNCLibUserContext, CNCLibUserContext>();

            Dependency.Container.RegisterMapper(new MapperConfiguration(cfg => { cfg.AddProfile<LogicAutoMapperProfile>(); }));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            string sqlConnectString = $"Data Source = cnclibdb.database.windows.net; Initial Catalog = CNCLibDb; Persist Security Info = True; User ID = {Xxx}; Password = {Yyy};";

            // Open Database here

            if (env.IsDevelopment())
            {
                sqlConnectString = null;
                GlobalDiagnosticsContext.Set("connectionString", MigrationCNCLibContext.ConnectString);
            }
            else
            {
                GlobalDiagnosticsContext.Set("connectionString", sqlConnectString);
            }

            Repository.SqlServer.MigrationCNCLibContext.InitializeDatabase(sqlConnectString, false, false);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "CNCLib API V1"); });

            app.UseCors("AllowAll");
            app.UseMvc();
        }

        public string Xxx => @"Herbert";
        public string Yyy => @"Edith1234";
    }
}