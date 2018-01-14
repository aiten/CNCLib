using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CNCLib.Logic;
using CNCLib.Logic.Contracts.DTO;
using CNCLib.ServiceProxy;
using CNCLib.WebAPI.Controllers;
using Framework.EF;
using Framework.Tools.Dependency;
using Framework.Tools.Pattern;
using Framework.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
            services.AddCors();

            services.AddMvc();

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "CNCLib API", Version = "v1" });
            });

            Dependency.Initialize(new AspNetDependencyProvider(services));

            Dependency.Container.RegisterTypesIncludingInternals(
                typeof(ServiceProxy.Logic.MachineService).Assembly,
                typeof(Repository.MachineRepository).Assembly,
                typeof(Logic.Client.DynItemController).Assembly,
                typeof(Logic.MachineController).Assembly);
            Dependency.Container.RegisterType<IUnitOfWork, UnitOfWork<Repository.Context.CNCLibContext>>();

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
            app.UseCors(option => option.AllowAnyMethod());
            app.UseCors(option => option.AllowAnyOrigin());
            app.UseCors(option => option.AllowAnyHeader());
            app.UseCors(option => option.AllowCredentials());

            string sqlconnectstring =
                @"Data Source = cnclibdb.database.windows.net; Initial Catalog = CNCLibDb2; Persist Security Info = True; User ID = Herbert; Password = Edith1234;";

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
            app.UseMvc();
        }
    }
}
