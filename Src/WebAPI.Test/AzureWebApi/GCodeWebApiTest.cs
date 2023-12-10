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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

using CNCLib.Logic.Abstraction.DTO;

using FluentAssertions;

using Xunit;

public class GCodeWebApiTest : AzureWebApiTest
{
    private readonly string api = "/api/GCode";

    [Fact]
    public async Task PutHpgl()
    {
        var client = GetHttpClient();

        var info = new LoadOptions { LoadType = LoadOptions.ELoadType.Hpgl };

        var ass     = Assembly.GetExecutingAssembly();
        var assPath = Path.GetDirectoryName(new Uri(ass.Location).LocalPath);

        info.FileName    = assPath + @"\TestData\heikes-mietzi.hpgl";
        info.FileContent = await File.ReadAllBytesAsync(info.FileName);

        var response = await client.PostAsJsonAsync(api, info);
        response.EnsureSuccessStatusCode();

        var gcode = await response.Content.ReadAsAsync<string[]>();

        gcode.Should().NotBeNull();
    }

    [Fact]
    public async Task PutImage()
    {
        var client = GetHttpClient();

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

        var ass     = Assembly.GetExecutingAssembly();
        var assPath = Path.GetDirectoryName(new Uri(ass.Location).LocalPath);

        info.FileName    = assPath + @"\TestData\Wendelin_Ait110.png";
        info.FileContent = await File.ReadAllBytesAsync(info.FileName);

        var response = await client.PostAsJsonAsync(api, info);
        response.EnsureSuccessStatusCode();

        var gcode = await response.Content.ReadAsAsync<string[]>();

        gcode.Should().NotBeNull();
    }

    public class CreateGCode
    {
        public int     LoadOptionsId { get; set; }
        public string? FileName      { get; set; }

        public byte[]? FileContent { get; set; }
    }

    private async Task<IEnumerable<LoadOptions>> GetAllLoadOptions()
    {
        var client   = GetHttpClient();
        var response = await client.GetAsync("/api/loadoptions");

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsAsync<IList<LoadOptions>>();
    }

    [Fact]
    public async Task PutImageWithStoredOptions()
    {
        var all = (await GetAllLoadOptions()).ToList();
        all.Should().HaveCountGreaterThan(0);

        var first = all.First(l => l.SettingName == "laser grave image");

        var client = GetHttpClient();

        var ass     = Assembly.GetExecutingAssembly();
        var assPath = Path.GetDirectoryName(new Uri(ass.Location).LocalPath);

        var input = new CreateGCode
        {
            LoadOptionsId = first.Id,
            FileName      = assPath + @"\TestData\Wendelin_Ait110.png"
        };

        input.FileContent = await File.ReadAllBytesAsync(input.FileName);

        var response = await client.PutAsJsonAsync(api, input);
        response.EnsureSuccessStatusCode();

        var gcode = await response.Content.ReadAsAsync<string[]>();

        gcode.Should().NotBeNull();
    }
}