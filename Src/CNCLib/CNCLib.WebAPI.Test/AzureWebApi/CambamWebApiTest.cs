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
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;

using CNCLib.Logic.Contract.DTO;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CNCLib.WebAPI.Test.AzureWebApi
{
    [TestClass]
    public class CambamWebApiTest : AzureWebApiTest
    {
        private readonly string api = "/api/Cambam";

        [TestMethod]
        public async Task PutHpgl()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(AzureUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var info = new LoadOptions { LoadType = LoadOptions.ELoadType.HPGL };

                Assembly ass     = Assembly.GetExecutingAssembly();
                string   assPath = Path.GetDirectoryName(ass.Location);

                info.FileName    = assPath + @"\TestData\heikes-mietzi.hpgl";
                info.FileContent = File.ReadAllBytes(info.FileName);

                HttpResponseMessage response = await client.PostAsJsonAsync(api, info);
                response.EnsureSuccessStatusCode();

                string cambam = await response.Content.ReadAsAsync<string>();

                cambam.Should().NotBeNull();
            }
        }
    }
}