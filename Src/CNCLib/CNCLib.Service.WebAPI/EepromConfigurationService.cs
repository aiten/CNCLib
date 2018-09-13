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

using System.Net.Http;
using System.Threading.Tasks;
using CNCLib.Logic.Contracts.DTO;
using CNCLib.Service.Contracts;

namespace CNCLib.Service.WebAPI
{
    public class EepromConfigurationService : ServiceBase, IEepromConfigurationService
    {
        protected override string Api => @"api/EepromConfiguration";

        public async Task<EepromConfiguration> CalculateConfig(EepromConfigurationInput param)
        {
            using (HttpClient client = CreateHttpClient())
            {
                string paramuri =
                    $"teeth={param.Teeth}&toothsizeInMm={param.ToothsizeinMm}&microsteps={param.Microsteps}&stepsPerRotation={param.StepsPerRotation}&estimatedRotationSpeed={param.EstimatedRotationSpeed}&timeToAcc={param.TimeToAcc}&timeToDec={param.TimeToDec}";
                HttpResponseMessage response = await client.GetAsync(Api + "/" + paramuri);
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