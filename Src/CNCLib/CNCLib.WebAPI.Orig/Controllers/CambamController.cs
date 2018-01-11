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
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Serialization;
using CNCLib.Logic.Contracts.DTO;
using CNCLib.ServiceProxy;
using CNCLib.WebAPI.Models;

namespace CNCLib.WebAPI.Controllers
{
    public class CambamController : ApiController
	{
        public CambamController(ILoadOptionsService loadOptionsService)
        {
            _loadOptionsService = loadOptionsService ?? throw new ArgumentNullException();
        }

        readonly ILoadOptionsService _loadOptionsService;

        //		[ActionName("")]
        public string Post([FromBody] LoadOptions input)
		{
			var load = GCodeLoadHelper.CallLoad(input.FileName, input.FileContent, input);
			var sw = new StringWriter();
			new XmlSerializer(typeof(GCode.CamBam.CamBam)).Serialize(sw, load.CamBam);
			return sw.ToString();
		}

		//		[ActionName("CreateGCode")]
		public async Task<string> Put([FromBody] CreateGCode input)
		{
			LoadOptions opt = await _loadOptionsService.Get(input.LoadOptionsId);
			var load = GCodeLoadHelper.CallLoad(input.FileName, input.FileContent, opt);
			var sw = new StringWriter();
			new XmlSerializer(typeof(GCode.CamBam.CamBam)).Serialize(sw, load.CamBam);
			return sw.ToString();
		}
	}
}
