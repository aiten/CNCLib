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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using CNCLib.Logic.Contract.DTO;

using FluentAssertions;

using Xunit;

namespace CNCLib.WebAPI.Test.AzureWebApi
{
    public class LoadOptionsWebApiTest : AzureWebApiTest
    {
        private readonly string api = "/api/loadoptions";

        [Fact]
        public async Task GetOption1()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(AzureUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(api + "/1");

                response.IsSuccessStatusCode.Should().BeTrue();

                if (response.IsSuccessStatusCode)
                {
                    LoadOptions l = await response.Content.ReadAsAsync<LoadOptions>();

                    l.Should().NotBeNull();
                }
            }
        }

        private async Task Cleanup(string settingname)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(AzureUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage responseGet = await client.GetAsync(api);

                var all = await responseGet.Content.ReadAsAsync<IEnumerable<LoadOptions>>();

                var setting = all.FirstOrDefault(s => s.SettingName == settingname);
                if (setting != null)
                {
                    await client.DeleteAsync($"{api}/{setting.Id}");
                }
            }
        }

        [Fact]
        public async Task CreateDeleteOption()
        {
            await Cleanup("Settingname");
            await Cleanup("ComHA");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(AzureUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var m = new LoadOptions { SettingName = "Settingname" };

                HttpResponseMessage response = await client.PostAsJsonAsync(api, m);
                response.IsSuccessStatusCode.Should().BeTrue();

                if (response.IsSuccessStatusCode)
                {
                    Uri newUrl = response.Headers.Location;

                    // HTTPGET again
                    HttpResponseMessage responseGet = await client.GetAsync(newUrl);
                    responseGet.IsSuccessStatusCode.Should().BeTrue();

                    if (responseGet.IsSuccessStatusCode)
                    {
                        LoadOptions mget = await responseGet.Content.ReadAsAsync<LoadOptions>();

                        mget.SettingName.Should().Be("Settingname");

                        // HTTP PUT
                        mget.SettingName = "ComHA";
                        var responsePut = await client.PutAsJsonAsync(newUrl, mget);

                        // HTTPGET again2
                        HttpResponseMessage responseGet2 = await client.GetAsync(newUrl);
                        responseGet2.IsSuccessStatusCode.Should().BeTrue();

                        if (responseGet2.IsSuccessStatusCode)
                        {
                            LoadOptions mget2 = await responseGet2.Content.ReadAsAsync<LoadOptions>();

                            mget2.SettingName.Should().Be("ComHA");
                        }

                        // HTTP DELETE
                        response = await client.DeleteAsync(newUrl);

                        // HTTPGET again3
                        HttpResponseMessage responseGet3 = await client.GetAsync(newUrl);
                        responseGet3.StatusCode.Should().Be(HttpStatusCode.NotFound);

                        if (responseGet3.IsSuccessStatusCode)
                        {
                            LoadOptions mget3 = await responseGet3.Content.ReadAsAsync<LoadOptions>();
                            mget3.Should().BeNull();
                        }
                    }
                }
            }
        }
    }
}