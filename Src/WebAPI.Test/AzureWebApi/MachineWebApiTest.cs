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

namespace CNCLib.WebAPI.Test.AzureWebApi;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using CNCLib.Logic.Abstraction.DTO;

using FluentAssertions;

using Xunit;

public class MachineWebApiTest : AzureWebApiTest
{
    private readonly string api = @"api/Machine";

    private async Task<IEnumerable<Machine>> GetAll()
    {
        var client   = GetHttpClient();
        var response = await client.GetAsync(api);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsAsync<IList<Machine>>();
    }

    [Fact]
    public async Task GetMachine1()
    {
        var all = (await GetAll()).ToList();
        all.Should().HaveCountGreaterThan(0);

        var first = all.First();

        var client = GetHttpClient();

        var response = await client.GetAsync($"{api}/{first.MachineId}");

        response.IsSuccessStatusCode.Should().BeTrue();

        if (response.IsSuccessStatusCode)
        {
            var m = await response.Content.ReadAsAsync<Machine>();
            m.MachineId.Should().Be(first.MachineId);
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
            MachineCommands     = Array.Empty<MachineCommand>(),
            MachineInitCommands = Array.Empty<MachineInitCommand>()
        };

        var response = await client.PostAsJsonAsync(api, m);
        response.IsSuccessStatusCode.Should().BeTrue();

        if (response.IsSuccessStatusCode)
        {
            var newMachineUri = response.Headers.Location;

            // HTTPGET again
            var responseGet = await client.GetAsync(newMachineUri);
            responseGet.IsSuccessStatusCode.Should().BeTrue();

            if (responseGet.IsSuccessStatusCode)
            {
                var mget = await responseGet.Content.ReadAsAsync<Machine>();

                mget.Name.Should().Be("MyUnitTest");

                // HTTP PUT
                mget.ComPort = "ComHA";
                var responsePut = await client.PutAsJsonAsync(newMachineUri, mget);

                // HTTPGET again2
                var responseGet2 = await client.GetAsync(newMachineUri);
                responseGet2.IsSuccessStatusCode.Should().BeTrue();

                if (responseGet2.IsSuccessStatusCode)
                {
                    var mget2 = await responseGet2.Content.ReadAsAsync<Machine>();

                    mget2.ComPort.Should().Be("ComHA");
                }

                // HTTP DELETE
                response = await client.DeleteAsync(newMachineUri);

                // HTTPGET again3
                var responseGet3 = await client.GetAsync(newMachineUri);
                responseGet3.StatusCode.Should().Be(HttpStatusCode.NotFound);

                if (responseGet3.IsSuccessStatusCode)
                {
                    var mget3 = await responseGet3.Content.ReadAsAsync<Machine>();
                    mget3.Should().BeNull();
                }
            }
        }
    }
}