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

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Serialization;
using CNCLib.GCode.Load;
using CNCLib.Logic.Contracts;
using CNCLib.Logic.Contracts.DTO;
using CNCLib.ServiceProxy;
using CNCLib.WebAPI.Models;
using Framework.Tools.Dependency;

namespace CNCLib.WebAPI.Controllers
{
	public class CambamController : ApiController
	{
		//		[ActionName("")]
		public string Post([FromBody] LoadOptions input)
		{
			var load = GCodeLoadHelper.CallLoad(input.FileName, input.FileContent, input);
			var sw = new StringWriter();
			new XmlSerializer(typeof(CNCLib.GCode.CamBam.CamBam)).Serialize(sw, load.CamBam);
			return sw.ToString();

		}

		//		[ActionName("CreateGCode")]
		public async Task<string> Put([FromBody] CreateGCode input)
		{
			using (var service = Dependency.Resolve<ILoadOptionsService>())
			{
				LoadOptions opt = await service.Get(input.LoadOptionsId);
				var load = GCodeLoadHelper.CallLoad(input.FileName, input.FileContent, opt);
				var sw = new StringWriter();
				new XmlSerializer(typeof(CNCLib.GCode.CamBam.CamBam)).Serialize(sw, load.CamBam);
				return sw.ToString();
			}
		}
	}
}
