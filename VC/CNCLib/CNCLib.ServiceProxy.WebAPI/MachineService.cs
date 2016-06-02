////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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


using System;
using System.Collections.Generic;
using CNCLib.Logic.Contracts.DTO;
using CNCLib.Logic.Contracts;
using Framework.Tools.Dependency;
using System.Net.Http;
using System.Threading.Tasks;

namespace CNCLib.ServiceProxy.WebAPI
{
	public class MachineService : ServiceBase, IMachineService
	{
		protected readonly string api = @"api/Machine";

		public async Task<int> AddAsync(Machine value)
		{
			using (var client = CreateHttpClient())
			{
				HttpResponseMessage response = await client.PostAsJsonAsync(api, value);

				if (response.IsSuccessStatusCode)
					return -1;

				return await response.Content.ReadAsAsync<int>();
			}
		}
		public int Add(Machine value)
		{
			var task = AddAsync(value);
			return task.ConfigureAwait(false).GetAwaiter().GetResult();

			//return Task.Run(() => AddAsync(value)).Result;
		}

		public async Task<Machine> DefaultMachineAsync()
		{
			return await GetAsync(-1);
		}
		public Machine DefaultMachine()
		{
			return Task.Run(() => DefaultMachineAsync()).Result;
		}

		public async Task DeleteAsync(Machine value)
		{
			using (var client = CreateHttpClient())
			{
				HttpResponseMessage response = await client.DeleteAsync(api + "/" + value.MachineID);

				if (response.IsSuccessStatusCode)
				{
					//return RedirectToAction("Index");
				}
				//return HttpNotFound();
			}
		}
		public void Delete(Machine value)
		{
			Task.Run(() => DeleteAsync(value)).Wait();
		}

		public async Task<Machine> GetAsync(int id)
		{
			using (var client = CreateHttpClient())
			{
				HttpResponseMessage response = await client.GetAsync(api + "/" + id);
				if (response.IsSuccessStatusCode)
				{
					Machine value = await response.Content.ReadAsAsync<Machine>();

					return value;
				}
			}
			return null;
		}

		public Machine Get(int id)
		{
			return Task.Run(() => GetAsync(id)).Result;

		}
		public async Task<IEnumerable<Machine>> GetAllAsync()
		{ 

			using (var client = CreateHttpClient())
			{
				HttpResponseMessage response = await client.GetAsync(api);
				if (response.IsSuccessStatusCode)
				{
					IEnumerable<Machine> machines = await response.Content.ReadAsAsync<IEnumerable<Machine>>();
					return machines;
				}
				return null;
			}
		}

		public IEnumerable<Machine> GetAll()
		{
			return Task.Run(() => GetAllAsync()).Result;
		}

		public async Task<int> GetDetaultMachineAsync()
		{
			return 1;
			//throw new NotImplementedException();
		}

		public int GetDetaultMachine()
		{
			return Task.Run(() => GetDetaultMachineAsync()).Result;
		}

		public async Task SetDetaultMachineAsync(int defaultMachineID)
		{
			throw new NotImplementedException();
		}
		public void SetDetaultMachine(int defaultMachineID)
		{
			Task.Run(() => SetDetaultMachineAsync(defaultMachineID)).Wait();
		}

		public async Task<int> UpdateAsync(Machine value)
		{
			using (var client = CreateHttpClient())
			{
				var response = await client.PutAsJsonAsync(api + "/" + value.MachineID, value);

				if (response.IsSuccessStatusCode)
				{
					return value.MachineID;
				}
				return -1;
			}
		}
		public int Update(Machine value)
		{
			return Task.Run(() => UpdateAsync(value)).Result;
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
//					_controller.Dispose();
//					_controller = null;
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~MachineRest() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion

	}
}
