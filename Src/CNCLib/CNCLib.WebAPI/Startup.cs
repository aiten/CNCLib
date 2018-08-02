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
using CNCLib.Repository.Context;
using CNCLib.WebAPI.Controllers;
using Framework.Contracts.Repository;
using Framework.EF;
using Framework.Tools.Dependency;
using Framework.Tools.Pattern;
using Framework.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));

            services.AddMvc().
                AddJsonOptions(options =>
                options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "CNCLib API", Version = "v1" });
            });

            Dependency.Initialize(new AspNetDependencyProvider(services));

            Dependency.Container.RegisterTypeScoped<CNCLibContext, CNCLibContext>();
            Dependency.Container.RegisterTypeScoped<IUnitOfWork, UnitOfWork<Repository.Context.CNCLibContext>>();

            Dependency.Container.RegisterTypesIncludingInternals(
                typeof(ServiceProxy.Logic.MachineService).Assembly,
                typeof(Repository.MachineRepository).Assembly,
                typeof(Logic.Client.DynItemController).Assembly,
                typeof(Logic.MachineController).Assembly);

            Dependency.Container.RegisterType<IRest<Logic.Contracts.DTO.Machine>, MachineRest>();
            Dependency.Container.RegisterType<IRest<Logic.Contracts.DTO.LoadOptions>, LoadInfoRest>();
            Dependency.Container.RegisterType<IRest<Logic.Contracts.DTO.Item>, ItemRest>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<LogicAutoMapperProfile>();
            });

            IMapper mapper = config.CreateMapper();
            Dependency.Container.RegisterInstance(mapper);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            string sqlconnectstring =
                @"Data Source = cnclibdb.database.windows.net; Initial Catalog = CNCLibDb; Persist Security Info = True; User ID = Herbert; Password = Edith1234;";

            // Open Database here

            if (env.IsDevelopment())
            {
                sqlconnectstring = null;
            }

            CNCLib.Repository.SqlServer.MigrationCNCLibContext.InitializeDatabase(sqlconnectstring, false);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CNCLib API V1");
            });

            app.UseCors("AllowAll");
            app.UseMvc();
        }
    }
}
