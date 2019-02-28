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

using CNCLib.Logic.Contract.DTO;
using CNCLib.Service.Contract;

using Framework.Tools.Uri;

namespace CNCLib.Service.WebAPI
{
    public class EepromConfigurationService : ServiceBase, IEepromConfigurationService
    {
        protected override string Api => @"api/EepromConfiguration";

        public async Task<EepromConfiguration> CalculateConfig(EepromConfigurationInput param)
        {
            using (HttpClient client = CreateHttpClient())
            {
                var paramUri = new UriQueryBuilder();
                paramUri.Add("teeth", param.Teeth)
                    .Add("toothSizeInMm",          param.ToothSizeInMm)
                    .Add("microSteps",             param.MicroSteps)
                    .Add("stepsPerRotation",       param.StepsPerRotation)
                    .Add("estimatedRotationSpeed", param.EstimatedRotationSpeed)
                    .Add("timeToAcc",              param.TimeToAcc)
                    .Add("timeToDec",              param.TimeToDec);

                HttpResponseMessage response = await client.GetAsync(UriPathBuilder.Build(Api,paramUri));
                if (response.IsSuccessStatusCode)
                {
                    EepromConfiguration value = await response.Content.ReadAsAsync<EepromConfiguration>();
                    return value;
                }
            }

            return null;
        }

        #region IDisposable Support

        #endregion
    }
}