////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using CNCLib.Logic.Contracts.DTO;
using System.Reflection;
using FluentAssertions;

namespace CNCLib.WebAPI.Tests.AzureWebApi
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

				Assembly ass = Assembly.GetExecutingAssembly();
				string asspath = Path.GetDirectoryName(ass.Location);

				info.FileName = asspath + @"\TestData\heikes-mietzi.hpgl";
				info.FileContent = File.ReadAllBytes(info.FileName);

				HttpResponseMessage response = await client.PostAsJsonAsync(api, info);
				response.EnsureSuccessStatusCode();

				string cambam = await response.Content.ReadAsAsync<string>();

				cambam.Should().NotBeNull();
			}
		}
/*
		[TestMethod]
		public async Task PutImage()
		{
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(AzureUrl);
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				LoadOptions info = new LoadOptions()
				{
					LoadType = LoadOptions.ELoadType.Image,
					AutoScale = true,
					AutoScaleSizeX = 100,
					AutoScaleSizeY = 100,
					MoveSpeed = 450,
					PenMoveType = LoadOptions.PenType.CommandString,
					ImageDPIX = 66.7m,
					ImageDPIY = 66.7m
				};

				Assembly ass = Assembly.GetExecutingAssembly();
				string asspath = Path.GetDirectoryName(ass.Location);

				info.FileName = asspath + @"\TestData\Wendelin_Ait110.png";
				info.FileContent = File.ReadAllBytes(info.FileName);

				HttpResponseMessage response = await client.PostAsJsonAsync(api, info);
				response.EnsureSuccessStatusCode();

				string[] gcode = await response.Content.ReadAsAsync<string[]>();

				Assert.IsNotNull(gcode);
			}
		}
*/
/*
		[TestMethod]
		public async Task PutImageWithStoredOptions()
		{
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(AzureUrl);
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				Assembly ass = Assembly.GetExecutingAssembly();
				string asspath = Path.GetDirectoryName(ass.Location);

				var input = new CreateGCode()
				{
					LoadOptionsId = 1,
					FileName = asspath + @"\TestData\Wendelin_Ait110.png"
				};

				input.FileContent = File.ReadAllBytes(input.FileName);

				HttpResponseMessage response = await client.PutAsJsonAsync(api, input);
				response.EnsureSuccessStatusCode();

				string[] gcode = await response.Content.ReadAsAsync<string[]>();

				Assert.IsNotNull(gcode);
			}
		}
*/
	}
}
