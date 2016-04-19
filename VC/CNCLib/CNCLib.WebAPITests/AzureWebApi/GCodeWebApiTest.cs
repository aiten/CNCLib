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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Framework.Test;
using System.Net.Http;
using System.Net.Http.Headers;
using CNCLib.Logic.Contracts.DTO;
using CNCLib.GCode.Load;

namespace CNCLib.WebAPITests.AzureWebApi
{
    [TestClass]
    public class GCodeWebApiTest : AzureWebApiTest
    {
        private readonly string api = "/api/GCode";

        [TestMethod]
        public async Task PutHpgl()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(AzureUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				LoadInfo info = new LoadInfo() { LoadType = LoadInfo.ELoadType.HPGL };

				info.FileName = @"c:\data\heikes-mietzi.hpgl";
				info.FileContent = File.ReadAllBytes(info.FileName);

				HttpResponseMessage response = await client.PostAsJsonAsync(api, info);
				response.EnsureSuccessStatusCode();

				string[] gcode = await response.Content.ReadAsAsync<string[]>();

				Assert.IsNotNull(gcode);
			}
		}

		[TestMethod]
		public async Task PutImage()
		{
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(AzureUrl);
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				LoadInfo info = new LoadInfo()
				{
					LoadType = LoadInfo.ELoadType.Image,
					AutoScale = true,
					AutoScaleSizeX = 100,
					AutoScaleSizeY = 100,
					MoveSpeed = 450,
					PenMoveType = LoadInfo.PenType.CommandString,
					ImageDPIX = 66.7m,
					ImageDPIY = 66.7m
				};

				info.FileName = @"c:\data\Wendelin_Ait110.png";
				info.FileContent = File.ReadAllBytes(info.FileName);

				HttpResponseMessage response = await client.PostAsJsonAsync(api, info);
				response.EnsureSuccessStatusCode();

				string[] gcode = await response.Content.ReadAsAsync<string[]>();

				Assert.IsNotNull(gcode);
			}
		}
	}
}
