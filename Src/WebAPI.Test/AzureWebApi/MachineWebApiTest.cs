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

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using CNCLib.Logic.Abstraction.DTO;

using FluentAssertions;

using Xunit;

namespace CNCLib.WebAPI.Test.AzureWebApi
{
    public class MachineWebApiTest : AzureWebApiTest
    {
        private readonly string api = @"api/Machine";

        [Fact]
        public async Task GetMachine1()
        {
            var client = GetHttpClient();

            HttpResponseMessage response = await client.GetAsync(api + "/1");

            response.IsSuccessStatusCode.Should().BeTrue();

            if (response.IsSuccessStatusCode)
            {
                Machine m = await response.Content.ReadAsAsync<Machine>();

                m.MachineId.Should().Be(1);
            }
        }

        [Fact]
        public async Task CreateDeleteMachine()
        {
            var client = GetHttpClient();

            var m = new Machine
            {
                Name                = "MyUnitTest",
                ComPort             = "comxx",
                MachineCommands     = new MachineCommand[0],
                MachineInitCommands = new MachineInitCommand[0]
            };

            HttpResponseMessage response = await client.PostAsJsonAsync(api, m);
            response.IsSuccessStatusCode.Should().BeTrue();

            if (response.IsSuccessStatusCode)
            {
                Uri newMachineUri = response.Headers.Location;

                // HTTPGET again
                HttpResponseMessage responseGet = await client.GetAsync(newMachineUri);
                responseGet.IsSuccessStatusCode.Should().BeTrue();

                if (responseGet.IsSuccessStatusCode)
                {
                    Machine mget = await responseGet.Content.ReadAsAsync<Machine>();

                    mget.Name.Should().Be("MyUnitTest");

                    // HTTP PUT
                    mget.ComPort = "ComHA";
                    var responsePut = await client.PutAsJsonAsync(newMachineUri, mget);

                    // HTTPGET again2
                    HttpResponseMessage responseGet2 = await client.GetAsync(newMachineUri);
                    responseGet2.IsSuccessStatusCode.Should().BeTrue();

                    if (responseGet2.IsSuccessStatusCode)
                    {
                        Machine mget2 = await responseGet2.Content.ReadAsAsync<Machine>();

                        mget2.ComPort.Should().Be("ComHA");
                    }

                    // HTTP DELETE
                    response = await client.DeleteAsync(newMachineUri);

                    // HTTPGET again3
                    HttpResponseMessage responseGet3 = await client.GetAsync(newMachineUri);
                    responseGet3.StatusCode.Should().Be(HttpStatusCode.NotFound);

                    if (responseGet3.IsSuccessStatusCode)
                    {
                        Machine mget3 = await responseGet3.Content.ReadAsAsync<Machine>();
                        mget3.Should().BeNull();
                    }
                }
            }
        }
    }
}