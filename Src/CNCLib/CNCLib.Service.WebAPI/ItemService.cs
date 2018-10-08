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
using System.Net.Http;
using System.Threading.Tasks;
using CNCLib.Logic.Contracts.DTO;
using CNCLib.Service.Contracts;

namespace CNCLib.Service.WebAPI
{
    public class ItemService : CRUDServiceBase<Item, int>, IItemService
    {
        protected override string Api            => @"api/Item";
        protected override int    GetKey(Item i) => i.ItemId;

        public async Task<Item> DefaultItem()
        {
            return await Get(-1);
        }

        public async Task<IEnumerable<Item>> GetByClassName(string classname)
        {
            using (HttpClient client = CreateHttpClient())
            {
//			    HttpResponseMessage response = await client.GetAsync(_api + "/" + classname);
                HttpResponseMessage response = await client.GetAsync(Api + "/?classname=" + classname);
                if (response.IsSuccessStatusCode)
                {
                    IEnumerable<Item> items = await response.Content.ReadAsAsync<IEnumerable<Item>>();
                    return items;
                }

                return null;
            }
        }
    }
}