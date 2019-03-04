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
using System.Threading.Tasks;
using System.Xml.Serialization;

using CNCLib.Logic.Abstraction;
using CNCLib.Logic.Abstraction.DTO;

using Microsoft.AspNetCore.Mvc;

using CNCLib.Shared;
using CNCLib.WebAPI.Models;

namespace CNCLib.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class CambamController : Controller
    {
        public CambamController(ILoadOptionsManager loadOptionsManager, ICNCLibUserContext userContext)
        {
            _loadOptionsManager = loadOptionsManager ?? throw new ArgumentNullException();
            _userContext        = userContext ?? throw new ArgumentNullException();
            ((CNCLibUserContext)_userContext).InitFromController(this);
        }

        readonly ILoadOptionsManager _loadOptionsManager;
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
            LoadOptions opt  = await _loadOptionsManager.Get(input.LoadOptionsId);
            var         load = GCodeLoadHelper.CallLoad(input.FileName, input.FileContent, opt);
            var         sw   = new StringWriter();
            new XmlSerializer(typeof(GCode.CamBam.CamBam)).Serialize(sw, load.CamBam);
            return sw.ToString();
        }
    }
}