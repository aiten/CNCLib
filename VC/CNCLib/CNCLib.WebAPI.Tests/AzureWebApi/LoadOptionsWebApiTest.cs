////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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
using CNCLib.GCode.Load;

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

                Assert.AreEqual(true, response.IsSuccessStatusCode);

                if (response.IsSuccessStatusCode)
                {
                    LoadInfo l = await response.Content.ReadAsAsync<LoadInfo>();

                    Assert.AreEqual(true,l.AutoScale);
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

                var m = new LoadInfo() { SettingName = "Settingname" };

                HttpResponseMessage response = await client.PostAsJsonAsync(api, m);
                Assert.AreEqual(true, response.IsSuccessStatusCode);

                if (response.IsSuccessStatusCode)
                {
                    Uri newmUrl = response.Headers.Location;

                    // HTTPGET again
                    HttpResponseMessage responseget = await client.GetAsync(newmUrl);
                    Assert.AreEqual(true, responseget.IsSuccessStatusCode);

                    if (responseget.IsSuccessStatusCode)
                    {
						LoadInfo mget = await responseget.Content.ReadAsAsync<LoadInfo>();

                        Assert.AreEqual("Settingname", mget.SettingName);

                        // HTTP PUT
                        mget.SettingName = "ComHA";
                        var responsePut = await client.PutAsJsonAsync(newmUrl, mget);

                        // HTTPGET again2
                        HttpResponseMessage responseget2 = await client.GetAsync(newmUrl);
                        Assert.AreEqual(true, responseget2.IsSuccessStatusCode);

                        if (responseget2.IsSuccessStatusCode)
                        {
							LoadInfo mget2 = await responseget2.Content.ReadAsAsync<LoadInfo>();

                            Assert.AreEqual("ComHA", mget2.SettingName);
                        }

                        // HTTP DELETE
                        response = await client.DeleteAsync(newmUrl);

                        // HTTPGET again3
                        HttpResponseMessage responseget3 = await client.GetAsync(newmUrl);
                        Assert.AreEqual(true, responseget3.IsSuccessStatusCode);

                        if (responseget2.IsSuccessStatusCode)
                        {
                            Machine mget3 = await responseget3.Content.ReadAsAsync<Machine>();
                            Assert.IsNull(mget3);
                        }
                    }
                }
            }
        }
    }
}
