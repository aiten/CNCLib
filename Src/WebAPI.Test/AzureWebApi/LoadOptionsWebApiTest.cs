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

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using CNCLib.Logic.Abstraction.DTO;

using FluentAssertions;

using Xunit;

public class LoadOptionsWebApiTest : AzureWebApiTest
{
    private readonly string api = "/api/loadoptions";

    private async Task<IEnumerable<LoadOptions>> GetAll()
    {
        var client   = GetHttpClient();
        var response = await client.GetAsync(api);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsAsync<IList<LoadOptions>>();
    }

    [Fact]
    public async Task GetOption1()
    {
        var all = await GetAll();
        all.Should().HaveCountGreaterThan(0);

        var first = all.First();

        var client = GetHttpClient();

        var response = await client.GetAsync($"{api}/{first.Id}");

        response.IsSuccessStatusCode.Should().BeTrue();

        if (response.IsSuccessStatusCode)
        {
            var l = await response.Content.ReadAsAsync<LoadOptions>();

            l.Should().NotBeNull();
        }
    }

    private async Task Cleanup(string settingName)
    {
        var client = GetHttpClient();

        var responseGet = await client.GetAsync(api);

        var all = await responseGet.Content.ReadAsAsync<IEnumerable<LoadOptions>>();

        var setting = all.FirstOrDefault(s => s.SettingName == settingName);
        if (setting != null)
        {
            await client.DeleteAsync($"{api}/{setting.Id}");
        }
    }

    [Fact]
    public async Task CreateDeleteOption()
    {
        await Cleanup("Settingname");
        await Cleanup("ComHA");

        var client = GetHttpClient();

        var m = new LoadOptions { SettingName = "Settingname" };

        var response = await client.PostAsJsonAsync(api, m);
        response.IsSuccessStatusCode.Should().BeTrue();

        if (response.IsSuccessStatusCode)
        {
            var newUri = response.Headers.Location;

            // HTTPGET again
            var responseGet = await client.GetAsync(newUri);
            responseGet.IsSuccessStatusCode.Should().BeTrue();

            if (responseGet.IsSuccessStatusCode)
            {
                var mget = await responseGet.Content.ReadAsAsync<LoadOptions>();

                mget.SettingName.Should().Be("Settingname");

                // HTTP PUT
                mget.SettingName = "ComHA";
                var responsePut = await client.PutAsJsonAsync(newUri, mget);

                // HTTPGET again2
                var responseGet2 = await client.GetAsync(newUri);
                responseGet2.IsSuccessStatusCode.Should().BeTrue();

                if (responseGet2.IsSuccessStatusCode)
                {
                    var mget2 = await responseGet2.Content.ReadAsAsync<LoadOptions>();

                    mget2.SettingName.Should().Be("ComHA");
                }

                // HTTP DELETE
                response = await client.DeleteAsync(newUri);

                // HTTPGET again3
                var responseGet3 = await client.GetAsync(newUri);
                responseGet3.StatusCode.Should().Be(HttpStatusCode.NotFound);

                if (responseGet3.IsSuccessStatusCode)
                {
                    var mget3 = await responseGet3.Content.ReadAsAsync<LoadOptions>();
                    mget3.Should().BeNull();
                }
            }
        }
    }
}