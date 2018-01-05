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
using System.Threading.Tasks;
using System.Net.Http;

namespace CNCLib.ServiceProxy.WebAPI
{
    public class ItemService : ServiceBase, IItemService
	{
		protected readonly string _api = @"api/Item";


		public async Task<int> Add(Item value)
		{
			using (HttpClient client = CreateHttpClient())
			{
				HttpResponseMessage response = await client.PostAsJsonAsync(_api, value);

				if (response.IsSuccessStatusCode)
					return -1;

				return await response.Content.ReadAsAsync<int>();
			}
		}

		public async Task<Item> DefaultItem()
		{
			return await Get(-1);
		}

		public async Task Delete(Item value)
		{
			using (HttpClient client = CreateHttpClient())
			{
				HttpResponseMessage response = await client.DeleteAsync(_api + "/" + value.ItemID);

				if (response.IsSuccessStatusCode)
				{
					//return RedirectToAction("Index");
				}
				//return HttpNotFound();
			}
		}

		public async Task<Item> Get(int id)
		{
			using (HttpClient client = CreateHttpClient())
			{
				HttpResponseMessage response = await client.GetAsync(_api + "/" + id);
				if (response.IsSuccessStatusCode)
				{
					Item value = await response.Content.ReadAsAsync<Item>();

					return value;
				}
			}
			return null;
		}

		public async Task<IEnumerable<Item>> GetAll()
		{
			using (HttpClient client = CreateHttpClient())
			{
				HttpResponseMessage response = await client.GetAsync(_api);
				if (response.IsSuccessStatusCode)
				{
					IEnumerable<Item> items = await response.Content.ReadAsAsync<IEnumerable<Item>>();
					return items;
				}
				return null;
			}
		}


		public async Task<IEnumerable<Item>> GetByClassName(string classname)
		{
			using (HttpClient client = CreateHttpClient())
			{
				HttpResponseMessage response = await client.GetAsync(_api + "/?classname=" + classname);
				if (response.IsSuccessStatusCode)
				{
					IEnumerable<Item> items = await response.Content.ReadAsAsync<IEnumerable<Item>>();
					return items;
				}
				return null;
			}
		}


		public async Task<int> Update(Item value)
		{
			using (HttpClient client = CreateHttpClient())
			{
				HttpResponseMessage response = await client.PutAsJsonAsync(_api + "/" + value.ItemID, value);

				if (response.IsSuccessStatusCode)
				{
					return value.ItemID;
				}
				return -1;
			}
		}

		#region IDisposable Support
		#endregion

	}
}
