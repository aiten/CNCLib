﻿/*
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
using System.Linq;
using System.Threading.Tasks;
using CNCLib.Joystick.Shared;
using CNCLib.Joystick.WebAPI.Hubs;
using CNCLib.Joystick.WebAPI.SerialPort;

using Framework.Arduino.SerialCommunication;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CNCLib.Joystick.WebAPI.Controllers
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
        public async Task<ActionResult<SerialPortDefinition>> Connect(int id, int? baudRate = null, bool? dtrIsReset = true, bool? resetOnConnect = false)
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

        #endregion
    }
}