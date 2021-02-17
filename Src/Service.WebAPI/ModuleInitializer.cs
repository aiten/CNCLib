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

namespace CNCLib.Service.WebAPI
{
    using System;
    using System.Net.Http;

    using CNCLib.Service.Abstraction;

    using Framework.Localization.Abstraction;
    using Framework.Service.WebAPI;
    using Framework.Startup.Abstraction;

    using Microsoft.Extensions.DependencyInjection;

    public class ModuleInitializer : IModuleInitializer
    {
        public Action<HttpClient> ConfigureHttpClient { get; set; }

        public void AddServices(IServiceCollection services)
        {
            AddConfigureClientBuilder(services.AddHttpClient<IMachineService, MachineService>(ConfigureHttpClient));
            AddConfigureClientBuilder(services.AddHttpClient<IEepromConfigurationService, EepromConfigurationService>(ConfigureHttpClient));
            AddConfigureClientBuilder(services.AddHttpClient<IItemService, ItemService>(ConfigureHttpClient));
            AddConfigureClientBuilder(services.AddHttpClient<ILoadOptionsService, LoadOptionsService>(ConfigureHttpClient));
            AddConfigureClientBuilder(services.AddHttpClient<IUserService, UserService>(ConfigureHttpClient));
            AddConfigureClientBuilder(services.AddHttpClient<IJoystickService, JoystickService>(ConfigureHttpClient));
        }

        private static IHttpClientBuilder AddConfigureClientBuilder(IHttpClientBuilder builder)
        {
            return builder.ConfigurePrimaryHttpMessageHandler(HttpClientHelper.CreateHttpClientHandlerIgnoreSSLCertificatesError);
        }

        public void AddTranslationResources(ILocalizationCollector localisation)
        {
        }
    }
}