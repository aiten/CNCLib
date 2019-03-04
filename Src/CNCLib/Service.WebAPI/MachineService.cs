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

using CNCLib.Logic.Abstraction.DTO;
using CNCLib.Service.Abstraction;

using Framework.Tools.Uri;

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
                HttpResponseMessage response = await client.GetAsync(UriPathBuilder.Build(Api + "/default"));
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
                HttpResponseMessage response = await client.GetAsync(UriPathBuilder.Build(Api + "/defaultmachine"));
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
                var paramUri = new UriQueryBuilder();
                paramUri.Add("id", id);
                HttpResponseMessage response = await client.PutAsJsonAsync(UriPathBuilder.Build(Api + "/defaultmachine", paramUri), "dummy");

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