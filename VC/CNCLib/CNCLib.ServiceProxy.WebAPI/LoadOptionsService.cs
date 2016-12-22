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


using System.Collections.Generic;
using CNCLib.Logic.Contracts.DTO;
using System.Threading.Tasks;
using System.Net.Http;

namespace CNCLib.ServiceProxy.WebAPI
{
    public class LoadOptionsService : ServiceBase, ILoadOptionsService
	{
		protected readonly string _api = @"api/LoadOptions";

		public async Task<int> Add(LoadOptions value)
		{
			using (HttpClient client = CreateHttpClient())
			{
				HttpResponseMessage response = await client.PostAsJsonAsync(_api, value);

				if (response.IsSuccessStatusCode)
					return -1;

				return await response.Content.ReadAsAsync<int>();
			}
		}

		public async Task Delete(LoadOptions value)
		{
			using (HttpClient client = CreateHttpClient())
			{
				HttpResponseMessage response = await client.DeleteAsync(_api + "/" + value.Id);

				if (response.IsSuccessStatusCode)
				{
					//return RedirectToAction("Index");
				}
				//return HttpNotFound();
			}
		}

		public async Task<LoadOptions> Get(int id)
		{
			using (HttpClient client = CreateHttpClient())
			{
				HttpResponseMessage response = await client.GetAsync(_api + "/" + id);
				if (response.IsSuccessStatusCode)
				{
					LoadOptions value = await response.Content.ReadAsAsync<LoadOptions>();

					return value;
				}
			}
			return null;
		}

		public async Task<IEnumerable<LoadOptions>> GetAll()
		{
			using (HttpClient client = CreateHttpClient())
			{
				HttpResponseMessage response = await client.GetAsync(_api);
				if (response.IsSuccessStatusCode)
				{
					IEnumerable<LoadOptions> values = await response.Content.ReadAsAsync<IEnumerable<LoadOptions>>();
					return values;
				}
				return null;
			}
		}

		public async Task<int> Update(LoadOptions value)
		{
			using (var client = CreateHttpClient())
			{
				HttpResponseMessage response = await client.PutAsJsonAsync(_api + "/" + value.Id, value);

				if (response.IsSuccessStatusCode)
				{
					return value.Id;
				}
				return -1;
			}
		}

		#region IDisposable Support
		private bool _disposedValue; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
//					_controller.Dispose();
//					_controller = null;
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				_disposedValue = true;
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
