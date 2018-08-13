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


using System.Collections.Generic;
using CNCLib.Logic.Contracts.DTO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CNCLib.ServiceProxy.WebAPI
{
    public class MachineService : CRUDServiceBase<Machine,int>, IMachineService
	{
	    protected override string Api => @"api/Item";
	    protected override int GetKey(Machine m) => m.MachineID; 

		public async Task<Machine> DefaultMachine()
		{
			return await Get(-1);
		}

		public async Task<int> GetDetaultMachine()
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

		public async Task SetDetaultMachine(int id)
		{
			using (HttpClient client = CreateHttpClient())
			{
				HttpResponseMessage response = await client.PutAsJsonAsync($"{Api}/defaultmachine?id={id}","dummy");

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
