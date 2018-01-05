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
using System.Threading.Tasks;
using System.Xml.Serialization;
using CNCLib.GCode.Commands;
using CNCLib.GCode.Load;
using CNCLib.Logic.Contracts.DTO;

namespace CNCLib.GCode
{
    public class GCodeLoad
	{
        public async Task<CommandList> Load(LoadOptions loadinfo, bool azure)
        {
            if (azure)
            {
                await LoadAzureAsync(loadinfo);
            }
            else
            {
                LoadLocal(loadinfo);
            }
            return Commands;
        }

        private readonly string webserverurl = @"http://cnclibapi.azurewebsites.net";
        private readonly string api = @"api/GCode";

        #region private
        private CommandList Commands { get; } = new CommandList();

        private void LoadLocal(LoadOptions loadinfo)
        {
            try
            {
                LoadBase load = LoadBase.Create(loadinfo);

                load.LoadOptions = loadinfo;
                load.Load();
                Commands.Clear();
                Commands.AddRange(load.Commands);
                if (!string.IsNullOrEmpty(loadinfo.GCodeWriteToFileName))
                {
                    string gcodeFileName = Environment.ExpandEnvironmentVariables(loadinfo.GCodeWriteToFileName);
                    WriteGCodeFile(gcodeFileName);
                    WriteCamBamFile(load, Path.GetDirectoryName(gcodeFileName) + @"\" + Path.GetFileNameWithoutExtension(gcodeFileName) + @".cb");
                    WriteImportInfoFile(load, Path.GetDirectoryName(gcodeFileName) + @"\" + Path.GetFileNameWithoutExtension(gcodeFileName) + @".hpgl");
                }
            }
            catch (Exception e)
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
                string[] gcode = await response.Content.ReadAsAsync<string[]>();

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
				Command last = null;
				var state = new CommandState();
				foreach (var r in Commands)
				{
					string[] cmds = r.GetGCodeCommands(last?.CalculatedEndPosition,state);
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
					Commands.ForEach(c => { if (!string.IsNullOrEmpty(c.ImportInfo)) writer.WriteLine(c.ImportInfo); });
				}
			}
		}

		private HttpClient CreateHttpClient()
		{
		    var client = new HttpClient {BaseAddress = new Uri(webserverurl)};
		    client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			return client;
		}

        #endregion
    }
}
