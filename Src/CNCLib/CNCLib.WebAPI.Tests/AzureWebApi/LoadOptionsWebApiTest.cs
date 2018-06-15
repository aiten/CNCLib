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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using CNCLib.Logic.Contracts.DTO;
using System.Net;
using FluentAssertions;

namespace CNCLib.WebAPI.Tests.AzureWebApi
{
	[TestClass]
    public class LoadOptionsWebApiTest : AzureWebApiTest
    {
        private readonly string api = "/api/loadoptions";

        [TestMethod]
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

        [TestMethod]
        public async Task CreateDeleteOption()
        {
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
                    Uri newmUrl = response.Headers.Location;

                    // HTTPGET again
                    HttpResponseMessage responseget = await client.GetAsync(newmUrl);
                    responseget.IsSuccessStatusCode.Should().BeTrue();

                    if (responseget.IsSuccessStatusCode)
                    {
						LoadOptions mget = await responseget.Content.ReadAsAsync<LoadOptions>();

                        mget.SettingName.Should().Be("Settingname");

                        // HTTP PUT
                        mget.SettingName = "ComHA";
                        var responsePut = await client.PutAsJsonAsync(newmUrl, mget);

                        // HTTPGET again2
                        HttpResponseMessage responseget2 = await client.GetAsync(newmUrl);
                        responseget2.IsSuccessStatusCode.Should().BeTrue();

                        if (responseget2.IsSuccessStatusCode)
                        {
							LoadOptions mget2 = await responseget2.Content.ReadAsAsync<LoadOptions>();

                            mget2.SettingName.Should().Be("ComHA");
                        }

                        // HTTP DELETE
                        response = await client.DeleteAsync(newmUrl);

                        // HTTPGET again3
                        HttpResponseMessage responseget3 = await client.GetAsync(newmUrl);
						responseget3.StatusCode.Should().Be(HttpStatusCode.NotFound);

						if (responseget2.IsSuccessStatusCode)
                        {
                            Machine mget3 = await responseget3.Content.ReadAsAsync<Machine>();
                            mget3.Should().BeNull();
                        }
                    }
                }
            }
        }
    }
}
