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

using CNCLib.Logic.Contract.DTO;
using CNCLib.Service.Contract;

namespace CNCLib.Service.WebAPI
{
    public class MachineService : CRUDServiceBase<Machine, int>, IMachineService
    {
        protected override string Api => @"api/Machine";
        protected override int GetKey(Machine m) => m.MachineId;

        public async Task<Machine> DefaultMachine()
        {
            using (HttpClient client = CreateHttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(Api + "/default");
                if (response.IsSuccessStatusCode)
                {
                    var value = await response.Content.ReadAsAsync<Machine>();

                    return value;
                }
            }

            return null;
        }

        public async Task<int> GetDefaultMachine()
        {
            using (HttpClient client = CreateHttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(Api + "/defaultmachine");
                if (response.IsSuccessStatusCode)
                {
                    int value = await response.Content.ReadAsAsync<int>();

                    return value;
                }
            }

            return -1;
        }

        public async Task SetDefaultMachine(int id)
        {
            using (HttpClient client = CreateHttpClient())
            {
                HttpResponseMessage response = await client.PutAsJsonAsync($"{Api}/defaultmachine?id={id}", "dummy");

                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
        }

        #region IDisposable Support

        #endregion
    }
}