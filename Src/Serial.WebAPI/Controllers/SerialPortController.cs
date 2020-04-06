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

using CNCLib.GCode.Draw;
using CNCLib.GCode.Load;
using CNCLib.Serial.Shared;
using CNCLib.Serial.WebAPI.Hubs;
using CNCLib.Serial.WebAPI.Models;
using CNCLib.Serial.WebAPI.SerialPort;

using Framework.Arduino.SerialCommunication;
using Framework.Drawing;
using Framework.WebAPI.Controller;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CNCLib.Serial.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class SerialPortController : Controller
    {
        private readonly IHubContext<CNCLibHub, ICNCLibHubClient> _hubContext;
        private static   IHubContext<CNCLibHub, ICNCLibHubClient> _myHubContext;

        public SerialPortController(IHubContext<CNCLibHub, ICNCLibHubClient> hubContext)
        {
            _hubContext   = hubContext;
            _myHubContext = _hubContext;
        }

        private SerialPortDefinition GetDefinition(SerialPortWrapper port)
        {
            return new SerialPortDefinition()
            {
                Id              = port.Id,
                PortName        = port.PortName,
                IsConnected     = port.IsConnected,
                IsAborted       = port.IsAborted,
                IsSingleStep    = port.IsSingleStep,
                CommandsInQueue = port.CommandsInQueue
            };
        }

        #region Query/Info

        [HttpGet]
        public async Task<IEnumerable<SerialPortDefinition>> Get(string portName = null)
        {
            var allPorts = SerialPortList.Ports.Select(GetDefinition);

            if (!string.IsNullOrEmpty(portName))
            {
                allPorts = allPorts.Where(port => 0 == string.Compare(port.PortName, portName, StringComparison.OrdinalIgnoreCase));
            }

            return await Task.FromResult(allPorts);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SerialPortDefinition>> Get(int id)
        {
            var port = await SerialPortList.GetPortAndRescan(id);
            if (port == null)
            {
                return NotFound();
            }

            return GetDefinition(port);
        }

        [HttpPost("refresh")]
        public async Task<IEnumerable<SerialPortDefinition>> Refresh()
        {
            SerialPortList.Refresh();
            return await Get();
        }

        #endregion

        #region Connect/Disconnect

        [HttpPost("{id:int}/connect")]
        public async Task<ActionResult<SerialPortDefinition>> Connect(int id, string commandPrefix = null, int? baudRate = null, bool? dtrIsReset = true, bool? resetOnConnect = false)
        {
            bool dtrIsResetN0     = dtrIsReset ?? true;
            bool resetOnConnectN0 = resetOnConnect ?? false;
            int  baudRateN0       = baudRate ?? 250000;

            var port = await SerialPortList.GetPortAndRescan(id);
            if (port == null)
            {
                return NotFound();
            }

            if (port.IsConnected)
            {
                if (port.Serial.BaudRate == baudRateN0 && resetOnConnectN0 == false)
                {
                    return Ok(GetDefinition(port));
                }

                await port.Serial.DisconnectAsync();
            }

            port.Serial.BaudRate       = baudRateN0;
            port.Serial.DtrIsReset     = dtrIsResetN0;
            port.Serial.ResetOnConnect = resetOnConnectN0;
            port.GCodeCommandPrefix    = commandPrefix ?? "";

            await port.Serial.ConnectAsync(port.PortName, null, null, null);

            await _hubContext.Clients.All.Connected(id);

            return Ok(GetDefinition(port));
        }

        [HttpPost("{id:int}/disconnect")]
        public async Task<ActionResult<SerialPortDefinition>> DisConnect(int id)
        {
            var port = await SerialPortList.GetPortAndRescan(id);
            if (port == null)
            {
                return NotFound();
            }

            await port.Serial.DisconnectAsync();
            port.Serial = null;

            await _hubContext.Clients.All.Disconnected(id);

            return Ok();
        }

        #endregion

        #region Send/Queue/Commands

        [HttpPost("{id:int}/queue")]
        public async Task<ActionResult<IEnumerable<SerialCommand>>> QueueCommand(int id, [FromBody] SerialCommands commands)
        {
            var port = await SerialPortList.GetPortAndRescan(id);

            if (port == null || commands == null || commands.Commands == null)
            {
                return NotFound();
            }

            var ret = await port.Serial.QueueCommandsAsync(commands.Commands);
            return Ok(ret);
        }

        [HttpPost("{id:int}/send")]
        public async Task<ActionResult<IEnumerable<SerialCommand>>> SendCommand(int id, [FromBody] SerialCommands commands)
        {
            var port = await SerialPortList.GetPortAndRescan(id);

            if (port == null || commands == null || commands.Commands == null)
            {
                return NotFound();
            }

            var ret = await port.Serial.SendCommandsAsync(commands.Commands, commands.TimeOut);
            return Ok(ret);
        }

        [HttpPost("{id:int}/sendWhileOk")]
        public async Task<ActionResult<IEnumerable<SerialCommand>>> SendWhileOkCommand(int id, [FromBody] SerialCommands commands)
        {
            var port = await SerialPortList.GetPortAndRescan(id);

            if (port == null || commands == null || commands.Commands == null)
            {
                return NotFound();
            }

            var ret = new List<SerialCommand>();
            foreach (var c in commands.Commands)
            {
                var result = await port.Serial.SendCommandsAsync(new[] { c }, commands.TimeOut);
                ret.AddRange(result);
                if (result.Any() && result.LastOrDefault().ReplyType != EReplyType.ReplyOK)
                {
                    break;
                }
            }

            return Ok(ret);
        }

        [HttpPost("{id:int}/abort")]
        public async Task<ActionResult> AbortCommand(int id)
        {
            var port = await SerialPortList.GetPortAndRescan(id);
            if (port == null)
            {
                return NotFound();
            }

            port.Serial.AbortCommands();
            return Ok();
        }

        [HttpPost("{id:int}/resume")]
        public async Task<ActionResult> ResumeCommand(int id)
        {
            var port = await SerialPortList.GetPortAndRescan(id);
            if (port == null)
            {
                return NotFound();
            }

            port.Serial.ResumeAfterAbort();
            return Ok();
        }

        [HttpPost("{id:int}/enablesinglestep")]
        public async Task<ActionResult> EnableSingleStepCommand(int id)
        {
            var port = await SerialPortList.GetPortAndRescan(id);
            if (port == null)
            {
                return NotFound();
            }

            port.Serial.Pause = true;
            return Ok();
        }

        [HttpPost("{id:int}/disbablesinglestep")]
        public async Task<ActionResult> DisableSingleStepCommand(int id)
        {
            var port = await SerialPortList.GetPortAndRescan(id);
            if (port == null)
            {
                return NotFound();
            }

            port.Serial.Pause = false;
            return Ok();
        }

        [HttpPost("{id:int}/singlestep")]
        public async Task<ActionResult> SingleStepCommand(int id)
        {
            var port = await SerialPortList.GetPortAndRescan(id);
            if (port == null)
            {
                return NotFound();
            }

            port.Serial.SendNext = true;
            return Ok();
        }

        #endregion

        #region History

        [HttpPost("{id:int}/history/clear")]
        public async Task<IActionResult> ClearCommandHistory(int id)
        {
            var port = await SerialPortList.GetPortAndRescan(id);
            if (port == null)
            {
                return NotFound();
            }

            port.Serial.ClearCommandHistory();
            return Ok();
        }

        [HttpGet("{id:int}/history")]
        public async Task<ActionResult<IEnumerable<SerialCommand>>> GetCommandHistory(int id, bool? sortDesc = null)
        {
            var port = await SerialPortList.GetPortAndRescan(id);
            if (port == null)
            {
                return NotFound();
            }

            var cmdList = port.Serial.CommandHistoryCopy;

            if (sortDesc ?? true)
            {
                cmdList = cmdList.OrderByDescending(h => h.SeqId).ToList();
            }

            return Ok(cmdList);
        }

        #endregion

        #region Render

        [HttpPut("{id:int}/render")]
        public async Task<IActionResult> Render(int id, [FromBody] PreviewGCode opt)
        {
            var port = await SerialPortList.GetPortAndRescan(id);
            if (port == null)
            {
                return NotFound();
            }

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

            if (!string.IsNullOrEmpty(opt.MachineColor)) gCodeDraw.MachineColor       = System.Drawing.ColorTranslator.FromHtml(opt.MachineColor);
            if (!string.IsNullOrEmpty(opt.LaserOnColor)) gCodeDraw.LaserOnColor       = System.Drawing.ColorTranslator.FromHtml(opt.LaserOnColor);
            if (!string.IsNullOrEmpty(opt.LaserOffColor)) gCodeDraw.LaserOffColor     = System.Drawing.ColorTranslator.FromHtml(opt.LaserOffColor);
            if (!string.IsNullOrEmpty(opt.CutColor)) gCodeDraw.CutColor               = System.Drawing.ColorTranslator.FromHtml(opt.CutColor);
            if (!string.IsNullOrEmpty(opt.CutDotColor)) gCodeDraw.CutDotColor         = System.Drawing.ColorTranslator.FromHtml(opt.CutDotColor);
            if (!string.IsNullOrEmpty(opt.CutEllipseColor)) gCodeDraw.CutEllipseColor = System.Drawing.ColorTranslator.FromHtml(opt.CutEllipseColor);
            if (!string.IsNullOrEmpty(opt.CutArcColor)) gCodeDraw.CutArcColor         = System.Drawing.ColorTranslator.FromHtml(opt.CutArcColor);
            if (!string.IsNullOrEmpty(opt.FastMoveColor)) gCodeDraw.FastMoveColor     = System.Drawing.ColorTranslator.FromHtml(opt.FastMoveColor);
            if (!string.IsNullOrEmpty(opt.HelpLineColor)) gCodeDraw.HelpLineColor     = System.Drawing.ColorTranslator.FromHtml(opt.HelpLineColor);

            var hisCommands = port.Serial.CommandHistoryCopy.OrderBy(x => x.SeqId).Select(c => c.CommandText);

            var load = new LoadGCode();
            load.Load(hisCommands.ToArray());
            var commands = load.Commands;
            var bitmap   = gCodeDraw.DrawToBitmap(commands);

            var memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, ImageFormat.Png);
            memoryStream.Position = 0;
            var fileName = "preview.png";
            return File(memoryStream, this.GetContentType(fileName), fileName);
        }

        [HttpGet("{id:int}/render")]
        public async Task<ActionResult<PreviewGCode>> RenderDefault(int id)
        {
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
                MachineColor    = System.Drawing.ColorTranslator.ToHtml(gCodeDraw.MachineColor),
                LaserOnColor    = System.Drawing.ColorTranslator.ToHtml(gCodeDraw.LaserOnColor),
                LaserOffColor   = System.Drawing.ColorTranslator.ToHtml(gCodeDraw.LaserOffColor),
                CutColor        = System.Drawing.ColorTranslator.ToHtml(gCodeDraw.CutColor),
                CutDotColor     = System.Drawing.ColorTranslator.ToHtml(gCodeDraw.CutDotColor),
                CutEllipseColor = System.Drawing.ColorTranslator.ToHtml(gCodeDraw.CutEllipseColor),
                CutArcColor     = System.Drawing.ColorTranslator.ToHtml(gCodeDraw.CutArcColor),
                FastMoveColor   = System.Drawing.ColorTranslator.ToHtml(gCodeDraw.FastMoveColor),
                HelpLineColor   = System.Drawing.ColorTranslator.ToHtml(gCodeDraw.HelpLineColor),
                Rotate3DAngle   = 0,
                Rotate3DVect    = new List<double> { 0.0, 0.0, 1.0 },
                RenderSizeX     = 800,
                RenderSizeY     = 800
            };

            return Ok(opt);
        }

        #endregion
    }
}