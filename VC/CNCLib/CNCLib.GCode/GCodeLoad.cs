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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CNCLib.GCode.Commands;
using CNCLib.GCode.Load;
using CNCLib.Logic.Contracts.DTO;

namespace CNCLib.GCode
{
	public class GCodeLoad
	{
		private void SaveGCode(string filename)
		{
			using (StreamWriter sw = new StreamWriter(filename))
			{
				Command last = null;
				foreach (Command r in Commands)
				{
					string[] cmds = r.GetGCodeCommands(last != null ? last.CalculatedEndPosition : null);
					if (cmds != null)
					{
						foreach (String str in cmds)
						{
							sw.WriteLine(str);
						}
					}
					last = r;
				}
			}
		}

		CommandList _commands = new CommandList();
		protected CommandList Commands { get { return _commands; } }

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
					SaveGCode(loadinfo.GCodeWriteToFileName);
				}
				WriteCamBam(load);

			}
			catch (Exception)
			{
				throw;
			}
		}

		private static void WriteCamBam(LoadBase load)
		{
			using (TextWriter writer = new StreamWriter(@"c:\tmp\CNCLib.cb"))
			{
				XmlSerializer x = new XmlSerializer(typeof(CamBam.CamBam));
				x.Serialize(writer, load.CamBam);
			}
/*
			using (TextReader reader = new StreamReader(@"c:\tmp\CNCLib.cb"))
			{
				XmlSerializer x = new XmlSerializer(typeof(CamBam.CamBam));
				var cb = x.Deserialize(reader);
				cb.ToString();
			}
*/
		}

		private readonly string webserverurl = @"http://cnclibapi.azurewebsites.net";
		private readonly string api = @"api/GCode";

		private System.Net.Http.HttpClient CreateHttpClient()
		{
			var client = new HttpClient();
			client.BaseAddress = new Uri(webserverurl);
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			return client;
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
						SaveGCode(info.GCodeWriteToFileName);
					}
					WriteCamBam(load);
				}
			}
		}
	}
}
