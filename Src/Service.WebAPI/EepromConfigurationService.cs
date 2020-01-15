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

using System.Net.Http;
using System.Threading.Tasks;

using CNCLib.Logic.Abstraction.DTO;
using CNCLib.Service.Abstraction;

using Framework.Pattern;
using Framework.Service.WebAPI;
using Framework.Service.WebAPI.Uri;

namespace CNCLib.Service.WebAPI
{
    public class EepromConfigurationService : ServiceBase, IEepromConfigurationService
    {
        private HttpClient _httpClient;

        public EepromConfigurationService(HttpClient httpClient)
        {
            BaseApi = @"api/EepromConfiguration";
            _httpClient = httpClient;
        }
        protected override IScope<HttpClient> CreateScope()
        {
            return new ScopeInstance<HttpClient>(_httpClient);
        }

        public async Task<EepromConfiguration> CalculateConfig(EepromConfigurationInput param)
        {
            using (var scope = CreateScope())
            {

                var paramUri = new UriQueryBuilder();
                paramUri.Add("teeth", param.Teeth)
                    .Add("toothSizeInMm",          param.ToothSizeInMm)
                    .Add("microSteps",             param.MicroSteps)
                    .Add("stepsPerRotation",       param.StepsPerRotation)
                    .Add("estimatedRotationSpeed", param.EstimatedRotationSpeed)
                    .Add("timeToAcc",              param.TimeToAcc)
                    .Add("timeToDec",              param.TimeToDec);

                HttpResponseMessage response = await scope.Instance.GetAsync(CreatePathBuilder().AddQuery(paramUri).Build());
                if (response.IsSuccessStatusCode)
                {
                    EepromConfiguration value = await response.Content.ReadAsAsync<EepromConfiguration>();
                    return value;
                }

                return null;
            }
        }
    }
}