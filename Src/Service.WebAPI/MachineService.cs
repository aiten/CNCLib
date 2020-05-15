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

using Framework.Service.WebAPI;
using Framework.Service.WebAPI.Uri;

namespace CNCLib.Service.WebAPI
{
    public class MachineService : CrudServiceBase<Machine, int>, IMachineService
    {
        public MachineService(HttpClient httpClient) : base(httpClient)
        {
            BaseApi = @"api/Machine";
        }

        protected override int GetKey(Machine m) => m.MachineId;

        public async Task<Machine> Default()
        {
            using (var scope = CreateScope())
            {
                var response = await scope.Instance.GetAsync(CreatePathBuilder().AddPath("default").Build());
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<Machine>();
            }
        }

        public async Task<int> GetDefault()
        {
            using (var scope = CreateScope())
            {
                var response = await scope.Instance.GetAsync(CreatePathBuilder().AddPath("defaultmachine").Build());
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<int>();
            }
        }

        public async Task SetDefault(int id)
        {
            using (var scope = CreateScope())
            {
                var paramUri = new UriQueryBuilder();
                paramUri.Add("id", id);
                var response = await scope.Instance.PutAsJsonAsync(
                    CreatePathBuilder().AddPath("defaultmachine").AddQuery(paramUri).Build(), "dummy");

                response.EnsureSuccessStatusCode();
            }
        }

        public async Task<string> TranslateJoystickMessage(int machineId, string joystickMessage)
        {
            using (var scope = CreateScope())
            {
                var paramUri = new UriQueryBuilder();
                paramUri.Add("message", joystickMessage);
                var response = await scope.Instance.GetAsync(
                    CreatePathBuilder().AddPath(machineId).AddPath("joystick").AddQuery(paramUri).Build());

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<string>();
            }
        }

        public string TranslateJoystickMessage(Machine machine, string joystickMessage)
        {
            throw new System.NotImplementedException();
        }
    }
}