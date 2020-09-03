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
    using System.Xml.Serialization;

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
                    WriteGCodeFile(gcodeFileName);
                    WriteCamBamFile(load, Path.GetDirectoryName(gcodeFileName) + @"\" + Path.GetFileNameWithoutExtension(gcodeFileName) + @".cb");
                    WriteImportInfoFile(load, Path.GetDirectoryName(gcodeFileName) + @"\" + Path.GetFileNameWithoutExtension(gcodeFileName) + @".hpgl");
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
                info.FileContent = File.ReadAllBytes(info.FileName);

                HttpResponseMessage response = await client.PostAsJsonAsync(api, info);
                string[]            gcode    = await response.Content.ReadAsAsync<string[]>();

                if (response.IsSuccessStatusCode)
                {
                    var load = new LoadGCode();
                    load.Load(gcode);
                    Commands.Clear();
                    Commands.AddRange(load.Commands);
                    if (!string.IsNullOrEmpty(info.GCodeWriteToFileName))
                    {
                        WriteGCodeFile(Environment.ExpandEnvironmentVariables(info.GCodeWriteToFileName));
                    }
                }
            }
        }

        private void WriteGCodeFile(string filename)
        {
            using (var sw = new StreamWriter(Environment.ExpandEnvironmentVariables(filename)))
            {
                Command last  = null;
                var     state = new CommandState();
                foreach (var r in Commands)
                {
                    string[] cmds = r.GetGCodeCommands(last?.CalculatedEndPosition, state);
                    if (cmds != null)
                    {
                        foreach (string str in cmds)
                        {
                            sw.WriteLine(str);
                        }
                    }

                    last = r;
                }
            }
        }

        private static void WriteCamBamFile(LoadBase load, string filename)
        {
            using (TextWriter writer = new StreamWriter(Environment.ExpandEnvironmentVariables(filename)))
            {
                var x = new XmlSerializer(typeof(CamBam.CamBam));
                x.Serialize(writer, load.CamBam);
            }
        }

        private void WriteImportInfoFile(LoadBase load, string filename)
        {
            if (Commands.Exists(c => !string.IsNullOrEmpty(c.ImportInfo)))
            {
                using (TextWriter writer = new StreamWriter(Environment.ExpandEnvironmentVariables(filename)))
                {
                    Commands.ForEach(
                        c =>
                        {
                            if (!string.IsNullOrEmpty(c.ImportInfo))
                            {
                                writer.WriteLine(c.ImportInfo);
                            }
                        });
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