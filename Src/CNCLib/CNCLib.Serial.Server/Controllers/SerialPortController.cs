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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CNCLib.Serial.Server.SerialPort;
using CNCLib.Serial.Shared;
using Framework.Arduino.SerialCommunication;
using Microsoft.AspNetCore.Mvc;

namespace CNCLib.Serial.Server.Controllers
{
    [Route("api/[controller]")]
    public class SerialPortController : Controller
	{
	    protected string CurrentUri => $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";

	    private SerialPortDefinition GetDefinition(SerialPortWrapper port)
	    {
	        return new SerialPortDefinition()
	        {
	            Id = port.Id,
	            PortName = port.PortName,
	            IsConnected = port.IsConnected,
	            IsAborted = port.IsAborted,
                IsSingleStep = port.IsSingleStep,
	            CommandsInQueue = port.CommandsInQueue,
            };
	    }

	    private SerialPortWrapper GetPort(int id)
	    {
	        return SerialPortList.GetPort(id);
	    }

	    #region Query/Info

        [HttpGet]
	    public async Task<IEnumerable<SerialPortDefinition>> Get()
        {
            return SerialPortList.Ports.Select((e) => GetDefinition(e));
        }

	    [HttpGet("{id:int}")]
	    public async Task<IActionResult> Get(int id)
	    {
	        var port = GetPort(id);
	        if (port == null)
	        {
	            return NotFound();
	        }
	        return Ok(GetDefinition(port));
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
	    public async Task<IActionResult> Connect(int id, int? baudrate=null,bool? resetOnConnect=true)
	    {
	        var port = GetPort(id);
	        if (port == null)
	        {
	            return NotFound();
	        }

	        port.Serial.BaudRate = baudrate ?? 250000;
	        port.Serial.ResetOnConnect = resetOnConnect ?? true;

            port.Serial.Connect(port.PortName);

	        return Ok(GetDefinition(port));
	    }

	    [HttpPost("{id:int}/disconnect")]
	    public async Task<IActionResult> DisConnect(int id)
	    {
	        var port = GetPort(id);
	        if (port == null)
	        {
	            return NotFound();
	        }

	        port.Serial.Disconnect();
	        port.Serial = null;

            return Ok();
	    }

        #endregion

        #region Send/Queue/Commands

        [HttpPost("{id:int}/queue")]
	    public async Task<IActionResult> QueueCommand(int id, [FromBody] SerialCommands commands)
	    {
	        var port = GetPort(id);

            if (port == null || commands == null || commands.Commands == null)
            {
                return NotFound();
            }

            var ret = await port.Serial.QueueCommandsAsync(commands.Commands);
            return Ok(ret);
	    }

	    [HttpPost("{id:int}/send")]
	    public async Task<IActionResult> SendCommand(int id, [FromBody] SerialCommands commands)
	    {
	        var port = GetPort(id);

	        if (port == null || commands==null || commands.Commands==null)
	        {
	            return NotFound();
	        }

            var ret = await port.Serial.SendCommandsAsync(commands.Commands, commands.TimeOut);
            return Ok(ret);
        }

	    [HttpPost("{id:int}/abort")]
	    public async Task<IActionResult> AbortCommand(int id)
	    {
	        var port = GetPort(id);
	        if (port == null)
	        {
	            return NotFound();
	        }

	        port.Serial.AbortCommands();
	        return Ok();
	    }

	    [HttpPost("{id:int}/resume")]
	    public async Task<IActionResult> ResumeCommand(int id)
	    {
	        var port = GetPort(id);
	        if (port == null)
	        {
	            return NotFound();
	        }

	        port.Serial.ResumeAfterAbort();
	        return Ok();
	    }

	    [HttpPost("{id:int}/enablesinglestep")]
	    public async Task<IActionResult> EnableSingleStepCommand(int id)
	    {
	        var port = GetPort(id);
	        if (port == null)
	        {
	            return NotFound();
	        }

	        port.Serial.Pause = true;
	        return Ok();
	    }

	    [HttpPost("{id:int}/disbablesinglestep")]
	    public async Task<IActionResult> DisableSingleStepCommand(int id)
	    {
	        var port = GetPort(id);
	        if (port == null)
	        {
	            return NotFound();
	        }

	        port.Serial.Pause = false;
	        return Ok();
	    }

	    [HttpPost("{id:int}/singlestep")]
	    public async Task<IActionResult> SingleStepCommand(int id)
	    {
	        var port = GetPort(id);
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
	        var port = GetPort(id);
	        if (port == null)
	        {
	            return NotFound();
	        }

	        port.Serial.ClearCommandHistory();
	        return Ok();
	    }

        [HttpGet("{id:int}/history")]
	    public async Task<IActionResult> GetCommandHistory(int id)
	    {
	        var port = GetPort(id);
	        if (port == null)
	        {
	            return NotFound();
	        }

	        var cmdlist = port.Serial.CommandHistoryCopy;
	        return Ok(cmdlist);
	    }

        #endregion
    }
}
