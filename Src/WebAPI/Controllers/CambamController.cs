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

namespace CNCLib.WebAPI.Controllers;

using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

using CNCLib.GCode.Generate.CamBam;
using CNCLib.Logic.Abstraction;
using CNCLib.Logic.Abstraction.DTO;

using Microsoft.AspNetCore.Mvc;

using CNCLib.Shared;
using CNCLib.WebAPI.Models;

using Microsoft.AspNetCore.Authorization;

[Authorize]
[Route("api/[controller]")]
public class CambamController : Controller
{
    private readonly GCodeLoadHelper _loadHelper;

    public CambamController(ILoadOptionsManager loadOptionsManager, GCodeLoadHelper loadHelper, ICNCLibUserContext userContext)
    {
        _loadOptionsManager = loadOptionsManager;
        _loadHelper         = loadHelper;
        _userContext        = userContext;
    }

    readonly ILoadOptionsManager _loadOptionsManager;
    readonly ICNCLibUserContext  _userContext;

    [HttpPost]
    public async Task<string> Post([FromBody] LoadOptions input)
    {
        var load = await _loadHelper.CallLoad(input, false);
        if (load != null)
        {
            var sw = new StringWriter();
            new XmlSerializer(typeof(CamBam)).Serialize(sw, load.CamBam);
            return sw.ToString();
        }

        return string.Empty;
    }

    [HttpPut]
    public async Task<string> Put([FromBody] CreateGCode input)
    {
        var opt = await _loadOptionsManager.GetAsync(input.LoadOptionsId);
        if (opt != null)
        {
            opt.FileName    = input.FileName ?? string.Empty;
            opt.FileContent = input.FileContent;
            var load = await _loadHelper.CallLoad(opt, false);
            if (load != null)
            {
                var sw = new StringWriter();
                new XmlSerializer(typeof(CamBam)).Serialize(sw, load.CamBam);
                return sw.ToString();
            }
        }

        return string.Empty;
    }
}