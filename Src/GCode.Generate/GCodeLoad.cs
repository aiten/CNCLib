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

namespace CNCLib.GCode.Generate
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using CNCLib.GCode.Generate.Commands;
    using CNCLib.GCode.Generate.Load;
    using CNCLib.Logic.Abstraction.DTO;

    public class GCodeLoad
    {
        public async Task<CommandList> Load(LoadOptions loadInfo, bool azure)
        {
            if (azure)
            {
                await LoadAzureAsync(loadInfo);
            }
            else
            {
                LoadLocal(loadInfo);
            }

            return Commands;
        }

        private readonly string webServerUri = @"https://cnclib.azurewebsites.net";
        private readonly string api          = @"api/GCode";

        #region private

        private CommandList Commands { get; } = new CommandList();

        private void LoadLocal(LoadOptions loadInfo)
        {
            try
            {
                LoadBase load = LoadBase.Create(loadInfo);

                load.LoadOptions = loadInfo;
                load.Load();
                Commands.Clear();
                Commands.AddRange(load.Commands);
                if (!string.IsNullOrEmpty(loadInfo.GCodeWriteToFileName))
                {
                    string gcodeFileName = Environment.ExpandEnvironmentVariables(loadInfo.GCodeWriteToFileName);
                    using (var sw = File.CreateText(gcodeFileName))
                    {
                        load.WriteGCodeFile(sw);
                    }

                    using (var sw = File.CreateText(Path.GetDirectoryName(gcodeFileName) + @"\" + Path.GetFileNameWithoutExtension(gcodeFileName) + @".cb"))
                    {
                        load.WriteCamBamFile(sw);
                    }

                    using (var sw = File.CreateText(Path.GetDirectoryName(gcodeFileName) + @"\" + Path.GetFileNameWithoutExtension(gcodeFileName) + @".hpgl"))
                    {
                        load.WriteImportInfoFile(sw);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task LoadAzureAsync(LoadOptions info)
        {
            using (var client = CreateHttpClient())
            {
                info.FileContent = await File.ReadAllBytesAsync(info.FileName);

                var response = await client.PostAsJsonAsync(api, info);
                var gcode    = await response.Content.ReadAsAsync<string[]>();

                if (response.IsSuccessStatusCode)
                {
                    var load = new LoadGCode();
                    load.Load(gcode);
                    Commands.Clear();
                    Commands.AddRange(load.Commands);
                    if (!string.IsNullOrEmpty(info.GCodeWriteToFileName))
                    {
                        using (var sw = File.CreateText(Environment.ExpandEnvironmentVariables(info.GCodeWriteToFileName)))
                        {
                            load.WriteGCodeFile(sw);
                        }
                    }
                }
            }
        }

        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient { BaseAddress = new Uri(webServerUri) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        #endregion
    }
}