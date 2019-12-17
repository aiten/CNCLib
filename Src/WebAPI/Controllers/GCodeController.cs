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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using CNCLib.GCode.Commands;
using CNCLib.GCode.Draw;
using CNCLib.GCode.Load;
using CNCLib.Logic.Abstraction;
using CNCLib.Logic.Abstraction.DTO;

using Microsoft.AspNetCore.Mvc;

using CNCLib.Shared;
using CNCLib.WebAPI.Models;

using Framework.Drawing;
using Framework.WebAPI.Controller;

using Microsoft.AspNetCore.Authorization;

namespace CNCLib.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class GCodeController : Controller
    {
        private readonly ILoadOptionsManager _loadOptionsManager;
        private readonly IUserFileManager    _fileManager;
        private readonly ICNCLibUserContext  _userContext;

        public GCodeController(ILoadOptionsManager loadOptionsManager, IUserFileManager fileManager, ICNCLibUserContext userContext)
        {
            _loadOptionsManager = loadOptionsManager;
            _fileManager        = fileManager;
            _userContext        = userContext;
        }

        [HttpPost]
        public async Task<IEnumerable<string>> Post([FromBody] LoadOptions input)
        {
            if (input.FileName.StartsWith(@"db:"))
            {
                input.FileName = input.FileName.Substring(3);
                var fileDto = await _fileManager.GetByName(input.FileName);
                input.FileContent = fileDto.Content;
            }

            return GCodeLoadHelper.CallLoad(input).Commands.ToStringList();
        }

        [HttpPut]
        public async Task<IEnumerable<string>> Put([FromBody] CreateGCode input)
        {
            LoadOptions opt = await _loadOptionsManager.Get(input.LoadOptionsId);
            opt.FileName    = input.FileName;
            opt.FileContent = input.FileContent;

            return GCodeLoadHelper.CallLoad(opt).Commands.ToStringList();
        }

        [HttpPut("render")]
        public async Task<IActionResult> Render([FromBody] PreviewGCode opt)
        {
            var gCodeDraw = new GCodeBitmapDraw()
            {
                SizeX      = opt.SizeX,
                SizeY      = opt.SizeY,
                SizeZ      = opt.SizeZ,
                RenderSize = new Size(opt.RenderSizeX, opt.RenderSizeY),
                OffsetX    = opt.OffsetX,
                OffsetY    = opt.OffsetY,
                OffsetZ    = opt.OffsetZ,

                Zoom       = opt.Zoom,
                CutterSize = opt.CutterSize,
                LaserSize  = opt.LaserSize,
                KeepRatio  = opt.KeepRatio
            };

            if (opt.Rotate3DVect != null && opt.Rotate3DVect.Count() == 3)
            {
                gCodeDraw.Rotate = new Rotate3D(opt.Rotate3DAngle, opt.Rotate3DVect.ToArray());
            }

            var load = new LoadGCode();
            load.Load(opt.Commands.ToArray());
            var commands = load.Commands;
            var bitmap   = gCodeDraw.DrawToBitmap(commands);

            var memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, ImageFormat.Png);
            memoryStream.Position = 0;
            var fileName = "preview.png";
            return File(memoryStream, this.GetContentType(fileName), fileName);
        }
    }
}