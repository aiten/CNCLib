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

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using CNCLib.GCode.Draw;
using CNCLib.GCode.Generate.Load;
using CNCLib.Logic.Abstraction;
using CNCLib.Logic.Abstraction.DTO;

using Microsoft.AspNetCore.Mvc;

using CNCLib.Shared;
using CNCLib.WebAPI.Models;

using Framework.Drawing;
using Framework.WebAPI.Controller;

using SkiaSharp;

[Route("api/[controller]")]
public class GCodeController : Controller
{
    private readonly ILoadOptionsManager _loadOptionsManager;
    private readonly GCodeLoadHelper     _loadHelper;
    private readonly IUserFileManager    _fileManager;
    private readonly ICNCLibUserContext  _userContext;

    public GCodeController(ILoadOptionsManager loadOptionsManager, IUserFileManager fileManager, GCodeLoadHelper loadHelper, ICNCLibUserContext userContext)
    {
        _loadOptionsManager = loadOptionsManager;
        _fileManager        = fileManager;
        _loadHelper         = loadHelper;
        _userContext        = userContext;
    }

    [HttpPost]
    public async Task<IEnumerable<string>> Post([FromBody] LoadOptions input)
    {
        if (input.FileName.StartsWith(@"db:"))
        {
            input.FileName = input.FileName.Substring(3);
            var fileDto = await _fileManager.GetByNameAsync(input.FileName);
            input.FileContent = fileDto.Content;
        }

        return (await _loadHelper.CallLoad(input, true)).Commands.ToStringList();
    }

    [HttpPut]
    public async Task<IEnumerable<string>> Put([FromBody] CreateGCode input)
    {
        var opt = await _loadOptionsManager.GetAsync(input.LoadOptionsId);
        opt.FileName    = input.FileName;
        opt.FileContent = input.FileContent;

        return (await _loadHelper.CallLoad(opt, true)).Commands.ToStringList();
    }

    [HttpPut("render")]
    public async Task<IActionResult> Render([FromBody] PreviewGCode opt)
    {
        await Task.CompletedTask;

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
            KeepRatio  = opt.KeepRatio,
        };

        if (!string.IsNullOrEmpty(opt.MachineColor)) gCodeDraw.MachineColor       = SKColor.Parse(opt.MachineColor);
        if (!string.IsNullOrEmpty(opt.LaserOnColor)) gCodeDraw.LaserOnColor       = SKColor.Parse(opt.LaserOnColor);
        if (!string.IsNullOrEmpty(opt.LaserOffColor)) gCodeDraw.LaserOffColor     = SKColor.Parse(opt.LaserOffColor);
        if (!string.IsNullOrEmpty(opt.CutColor)) gCodeDraw.CutColor               = SKColor.Parse(opt.CutColor);
        if (!string.IsNullOrEmpty(opt.CutDotColor)) gCodeDraw.CutDotColor         = SKColor.Parse(opt.CutDotColor);
        if (!string.IsNullOrEmpty(opt.CutEllipseColor)) gCodeDraw.CutEllipseColor = SKColor.Parse(opt.CutEllipseColor);
        if (!string.IsNullOrEmpty(opt.CutArcColor)) gCodeDraw.CutArcColor         = SKColor.Parse(opt.CutArcColor);
        if (!string.IsNullOrEmpty(opt.FastMoveColor)) gCodeDraw.FastMoveColor     = SKColor.Parse(opt.FastMoveColor);
        if (!string.IsNullOrEmpty(opt.HelpLineColor)) gCodeDraw.HelpLineColor     = SKColor.Parse(opt.HelpLineColor);

        if (opt.Rotate3DVect != null && opt.Rotate3DVect.Count() == 3)
        {
            gCodeDraw.Rotate = new Rotate3D(opt.Rotate3DAngle, opt.Rotate3DVect.ToArray());
        }

        var load = new LoadGCode();
        load.Load(opt.Commands.ToArray());
        var commands = load.Commands;
        var bitmap   = gCodeDraw.DrawToBitmap(commands);

        var memoryStream = new MemoryStream();
        bitmap.Save(memoryStream, SKEncodedImageFormat.Png);
        memoryStream.Position = 0;
        var fileName = "preview.png";
        return File(memoryStream, this.GetContentType(fileName), fileName);
    }

    [HttpGet("render")]
    public async Task<ActionResult<PreviewGCode>> RenderDefault()
    {
        await Task.CompletedTask;

        var gCodeDraw = new GCodeBitmapDraw();
        var opt = new PreviewGCode()
        {
            SizeX           = 200,
            SizeY           = 200,
            SizeZ           = 200,
            KeepRatio       = true,
            Zoom            = 1.0,
            OffsetX         = 0,
            OffsetY         = 0,
            OffsetZ         = 0,
            CutterSize      = gCodeDraw.CutterSize,
            LaserSize       = gCodeDraw.LaserSize,
            MachineColor    = gCodeDraw.MachineColor.ToString(),
            LaserOnColor    = gCodeDraw.LaserOnColor.ToString(),
            LaserOffColor   = gCodeDraw.LaserOffColor.ToString(),
            CutColor        = gCodeDraw.CutColor.ToString(),
            CutDotColor     = gCodeDraw.CutDotColor.ToString(),
            CutEllipseColor = gCodeDraw.CutEllipseColor.ToString(),
            CutArcColor     = gCodeDraw.CutArcColor.ToString(),
            FastMoveColor   = gCodeDraw.FastMoveColor.ToString(),
            HelpLineColor   = gCodeDraw.HelpLineColor.ToString(),
            Rotate3DAngle   = 0,
            Rotate3DVect    = new List<double> { 0.0, 0.0, 1.0 },
            RenderSizeX     = 800,
            RenderSizeY     = 800
        };

        return Ok(opt);
    }
}