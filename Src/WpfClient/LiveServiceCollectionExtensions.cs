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

using CNCLib.Service.Abstraction;

using Framework.Dependency;
using Framework.Pattern;

using Microsoft.Extensions.DependencyInjection;

namespace CNCLib.WpfClient
{
    public static class LiveServiceCollectionExtensions
    {
        public static IServiceCollection AddCNCLibWpf(this IServiceCollection services)
        {
            services.AddSingleton<Global>()
                .AddTransient<IFactory<IJoystickService>, FactoryResolve<IJoystickService>>()
                .AddTransient<IFactory<IMachineService>, FactoryResolve<IMachineService>>()
                .AddTransient<IFactory<ILoadOptionsService>, FactoryResolve<ILoadOptionsService>>()
                .AddTransient<IFactory<IJoystickService>, FactoryResolve<IJoystickService>>()
                .AddTransient<IFactory<IUserService>, FactoryResolve<IUserService>>()
                .AddAssemblyByName(
                    n => n.EndsWith("ViewModel"),
                    ServiceLifetime.Transient,
                    typeof(ViewModels.MachineViewModel).Assembly,
                    typeof(GCode.GUI.ViewModels.LoadOptionViewModel).Assembly);

            return services;
        }
    }
}