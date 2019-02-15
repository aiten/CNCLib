/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2019 Herbert Aitenbichler

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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using CNCLib.Logic.Contract.DTO;

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
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(AzureUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(api + "/1");

                response.IsSuccessStatusCode.Should().BeTrue();

                if (response.IsSuccessStatusCode)
                {
                    Machine m = await response.Content.ReadAsAsync<Machine>();

                    m.MachineId.Should().Be(1);
                }
            }
        }

        [Fact]
        public async Task CreateDeleteMachine()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(AzureUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

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
}