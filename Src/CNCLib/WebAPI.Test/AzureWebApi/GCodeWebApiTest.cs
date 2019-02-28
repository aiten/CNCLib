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
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;

using CNCLib.Logic.Contract.DTO;

using FluentAssertions;

using Xunit;

namespace CNCLib.WebAPI.Test.AzureWebApi
{
    public class GCodeWebApiTest : AzureWebApiTest
    {
        private readonly string api = "/api/GCode";

        [Fact]
        public async Task PutHpgl()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(AzureUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var info = new LoadOptions { LoadType = LoadOptions.ELoadType.HPGL };

                Assembly ass     = Assembly.GetExecutingAssembly();
                string   assPath = Path.GetDirectoryName(new Uri(ass.EscapedCodeBase).LocalPath);

                info.FileName    = assPath + @"\TestData\heikes-mietzi.hpgl";
                info.FileContent = File.ReadAllBytes(info.FileName);

                HttpResponseMessage response = await client.PostAsJsonAsync(api, info);
                response.EnsureSuccessStatusCode();

                string[] gcode = await response.Content.ReadAsAsync<string[]>();

                gcode.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task PutImage()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(AzureUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var info = new LoadOptions
                {
                    LoadType       = LoadOptions.ELoadType.Image,
                    AutoScale      = true,
                    AutoScaleSizeX = 100,
                    AutoScaleSizeY = 100,
                    MoveSpeed      = 450,
                    PenMoveType    = LoadOptions.PenType.CommandString,
                    ImageDPIX      = 66.7m,
                    ImageDPIY      = 66.7m
                };

                Assembly ass     = Assembly.GetExecutingAssembly();
                string   assPath = Path.GetDirectoryName(new Uri(ass.EscapedCodeBase).LocalPath);

                info.FileName    = assPath + @"\TestData\Wendelin_Ait110.png";
                info.FileContent = File.ReadAllBytes(info.FileName);

                HttpResponseMessage response = await client.PostAsJsonAsync(api, info);
                response.EnsureSuccessStatusCode();

                string[] gcode = await response.Content.ReadAsAsync<string[]>();

                gcode.Should().NotBeNull();
            }
        }

        public class CreateGCode
        {
            public int    LoadOptionsId { get; set; }
            public string FileName      { get; set; }

            public byte[] FileContent { get; set; }
        }

        [Fact]
        public async Task PutImageWithStoredOptions()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(AzureUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Assembly ass     = Assembly.GetExecutingAssembly();
                string   assPath = Path.GetDirectoryName(new Uri(ass.EscapedCodeBase).LocalPath);

                var input = new CreateGCode
                {
                    LoadOptionsId = 3,
                    FileName      = assPath + @"\TestData\Wendelin_Ait110.png"
                };

                input.FileContent = File.ReadAllBytes(input.FileName);

                HttpResponseMessage response = await client.PutAsJsonAsync(api, input);
                response.EnsureSuccessStatusCode();

                string[] gcode = await response.Content.ReadAsAsync<string[]>();

                gcode.Should().NotBeNull();
            }
        }
    }
}