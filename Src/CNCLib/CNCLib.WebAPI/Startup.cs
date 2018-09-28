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

using AutoMapper;
using CNCLib.Logic;
using CNCLib.Logic.Manager;
using CNCLib.Repository.Context;
using CNCLib.Service.Logic;
using CNCLib.Shared;
using Framework.Contracts.Repository;
using Framework.Contracts.Shared;
using Framework.Repository;
using Framework.Tools;
using Framework.Tools.Dependency;
using Framework.Web;
using Framework.Web.Filter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

namespace CNCLib.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

            services.AddTransient<UnhandledExceptionFilter>();
            services.AddTransient<ValidateRequestDataFilter>();
            services.AddMvc(options =>
                {
                    options.Filters.AddService<ValidateRequestDataFilter>();
                    options.Filters.AddService<UnhandledExceptionFilter>();
                }).
                SetCompatibilityVersion(CompatibilityVersion.Version_2_1).
                AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new Info { Title = "CNCLib API", Version = "v1" }); });

            Dependency.Initialize(new AspNetDependencyProvider(services));

            Dependency.Container.RegisterType<ICurrentDateTime, CurrentDateTime>();

            Dependency.Container.RegisterTypeScoped<CNCLibContext, CNCLibContext>();
            Dependency.Container.RegisterTypeScoped<IUnitOfWork, UnitOfWork<CNCLibContext>>();
            Dependency.Container.RegisterTypeScoped<ICNCLibUserContext, CNCLibUserContext>();

            Dependency.Container.RegisterTypesIncludingInternals(typeof(MachineService).Assembly, typeof(Repository.MachineRepository).Assembly, typeof(Logic.Client.DynItemController).Assembly,
                                                                 typeof(MachineManager).Assembly);


            var config = new MapperConfiguration(cfg => { cfg.AddProfile<LogicAutoMapperProfile>(); });

            IMapper mapper = config.CreateMapper();
            Dependency.Container.RegisterInstance(mapper);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            string sqlconnectstring = $"Data Source = cnclibdb.database.windows.net; Initial Catalog = CNCLibDb; Persist Security Info = True; User ID = {Xxx}; Password = {Yyy};";

            // Open Database here

            if (env.IsDevelopment())
            {
                sqlconnectstring = null;
            }

            Repository.SqlServer.MigrationCNCLibContext.InitializeDatabase(sqlconnectstring, false, false);

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