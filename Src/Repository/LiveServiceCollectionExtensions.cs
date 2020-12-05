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

namespace CNCLib.Repository
{
    using System;

    using CNCLib.Repository.Context;

    using Framework.Dependency;
    using Framework.Repository;
    using Framework.Repository.Abstraction;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public static class LiveServiceCollectionExtensions
    {
        public static IServiceCollection AddRepository(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction)
        {
            var options = new DbContextOptionsBuilder<CNCLibContext>();
            optionsAction(options);

            services.AddSingleton<DbContextOptions<CNCLibContext>>(options.Options);
            services.AddScoped<CNCLibContext, CNCLibContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork<CNCLibContext>>();

            services.AddAssemblyIncludingInternals(ServiceLifetime.Transient, typeof(Repository.MachineRepository).Assembly);
            return services;
        }
    }
}