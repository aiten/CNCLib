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
using System.Xml.Serialization;

using CNCLib.Logic.Contract.DTO;
using CNCLib.Service.Contract;

using Microsoft.AspNetCore.Mvc;

using CNCLib.Shared;
using CNCLib.WebAPI.Models;

namespace CNCLib.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class CambamController : Controller
    {
        public CambamController(ILoadOptionsService loadOptionsService, ICNCLibUserContext userContext)
        {
            _loadOptionsService = loadOptionsService ?? throw new ArgumentNullException();
            _userContext        = userContext ?? throw new ArgumentNullException();
            ((CNCLibUserContext)_userContext).InitFromController(this);
        }

        readonly ILoadOptionsService _loadOptionsService;
        readonly ICNCLibUserContext  _userContext;

        [HttpPost]
        public string Post([FromBody] LoadOptions input)
        {
            var load = GCodeLoadHelper.CallLoad(input.FileName, input.FileContent, input);
            var sw   = new StringWriter();
            new XmlSerializer(typeof(GCode.CamBam.CamBam)).Serialize(sw, load.CamBam);
            return sw.ToString();
        }

        [HttpPut]
        public async Task<string> Put([FromBody] CreateGCode input)
        {
            LoadOptions opt  = await _loadOptionsService.Get(input.LoadOptionsId);
            var         load = GCodeLoadHelper.CallLoad(input.FileName, input.FileContent, opt);
            var         sw   = new StringWriter();
            new XmlSerializer(typeof(GCode.CamBam.CamBam)).Serialize(sw, load.CamBam);
            return sw.ToString();
        }
    }
}