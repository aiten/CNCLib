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
using System.Reflection;
using System.Threading.Tasks;

using CNCLib.Logic.Abstraction.DTO;

using FluentAssertions;

using Xunit;

namespace CNCLib.WebAPI.Test.AzureWebApi
{
    public class CambamWebApiTest : AzureWebApiTest
    {
        private readonly string api = "/api/Cambam";

        [Fact]
        public async Task PutHpgl()
        {
            var client = GetHttpClient();

            var info = new LoadOptions { LoadType = LoadOptions.ELoadType.Hpgl };

            Assembly ass     = Assembly.GetExecutingAssembly();
            string   assPath = Path.GetDirectoryName(new Uri(ass.EscapedCodeBase).LocalPath);

            info.FileName    = assPath + @"\TestData\heikes-mietzi.hpgl";
            info.FileContent = File.ReadAllBytes(info.FileName);

            HttpResponseMessage response = await client.PostAsJsonAsync(api, info);
            response.EnsureSuccessStatusCode();

            string cambam = await response.Content.ReadAsAsync<string>();

            cambam.Should().NotBeNull();
        }
    }
}