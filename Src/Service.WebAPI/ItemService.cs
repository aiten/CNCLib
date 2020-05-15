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

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using CNCLib.Logic.Abstraction.DTO;
using CNCLib.Service.Abstraction;

using Framework.Service.WebAPI;
using Framework.Service.WebAPI.Uri;

namespace CNCLib.Service.WebAPI
{
    public class ItemService : CrudServiceBase<Item, int>, IItemService
    {
        protected override int GetKey(Item i) => i.ItemId;

        public ItemService(HttpClient httpClient) : base(httpClient)
        {
            BaseApi = @"api/Item";
        }

        public async Task<IEnumerable<Item>> GetByClassName(string classname)
        {
            using (var scope = CreateScope())
            {
                var paramUri = new UriQueryBuilder();
                paramUri.Add("classname", classname);

                var response = await scope.Instance.GetAsync(CreatePathBuilder().AddQuery(paramUri).Build());
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<IEnumerable<Item>>();
            }
        }
    }
}