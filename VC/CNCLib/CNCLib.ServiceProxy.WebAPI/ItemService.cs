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
using System.Threading.Tasks;
using System.Net.Http;

namespace CNCLib.ServiceProxy.WebAPI
{
	public class ItemService : ServiceBase, IItemService
	{
		protected readonly string api = @"api/Item";


		public async Task<int> AddAsync(Item value)
		{
			using (var client = CreateHttpClient())
			{
				HttpResponseMessage response = await client.PostAsJsonAsync(api, value);

				if (response.IsSuccessStatusCode)
					return -1;

				return await response.Content.ReadAsAsync<int>();
			}
		}
		public int Add(Item value)
		{
			var task = AddAsync(value);
			return task.ConfigureAwait(false).GetAwaiter().GetResult();

			//return Task.Run(() => AddAsync(value)).Result;
		}

		public async Task<Item> DefaultItemAsync()
		{
			return await GetAsync(-1);
		}
		public Item DefaultItem()
		{
			return Task.Run(() => DefaultItemAsync()).Result;
		}

		public async Task DeleteAsync(Item value)
		{
			using (var client = CreateHttpClient())
			{
				HttpResponseMessage response = await client.DeleteAsync(api + "/" + value.ItemID);

				if (response.IsSuccessStatusCode)
				{
					//return RedirectToAction("Index");
				}
				//return HttpNotFound();
			}
		}
		public void Delete(Item value)
		{
			Task.Run(() => DeleteAsync(value)).GetAwaiter().GetResult();
		}

		public async Task<Item> GetAsync(int id)
		{
			using (var client = CreateHttpClient())
			{
				HttpResponseMessage response = await client.GetAsync(api + "/" + id);
				if (response.IsSuccessStatusCode)
				{
					Item value = await response.Content.ReadAsAsync<Item>();

					return value;
				}
			}
			return null;
		}

		public Item Get(int id)
		{
			return Task.Run(() => GetAsync(id)).Result;

		}
		public async Task<IEnumerable<Item>> GetAllAsync()
		{

			using (var client = CreateHttpClient())
			{
				HttpResponseMessage response = await client.GetAsync(api);
				if (response.IsSuccessStatusCode)
				{
					IEnumerable<Item> Items = await response.Content.ReadAsAsync<IEnumerable<Item>>();
					return Items;
				}
				return null;
			}
		}

		public IEnumerable<Item> GetAll()
		{
			return Task.Run(() => GetAllAsync()).Result;
		}

		public async Task<IEnumerable<Item>> GetByClassNameAsync(string classname)
		{

			using (var client = CreateHttpClient())
			{
				HttpResponseMessage response = await client.GetAsync(api + "/?classname=" + classname);
				if (response.IsSuccessStatusCode)
				{
					IEnumerable<Item> Items = await response.Content.ReadAsAsync<IEnumerable<Item>>();
					return Items;
				}
				return null;
			}
		}

		public IEnumerable<Item> GetByClassName(string classname)
		{
			return Task.Run(() => GetByClassNameAsync(classname)).Result;
		}


		public async Task<int> UpdateAsync(Item value)
		{
			using (var client = CreateHttpClient())
			{
				var response = await client.PutAsJsonAsync(api + "/" + value.ItemID, value);

				if (response.IsSuccessStatusCode)
				{
					return value.ItemID;
				}
				return -1;
			}
		}
		public int Update(Item value)
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
					//_controller.Dispose();
					//_controller = null;
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
