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
using CNCLib.Serial.Server.Hubs;
using CNCLib.Serial.Server.SerialPort;
using CNCLib.Serial.Shared;
using Framework.Arduino.SerialCommunication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CNCLib.Serial.Server.Controllers
{
    [Route("api/[controller]")]
    public class SerialPortController : Controller
	{
        protected string CurrentUri => $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";

	    private IHubContext<CNCLibHub> _hubcontext;
	    static public IHubContext<CNCLibHub> _myhubcontext;

        public SerialPortController(IHubContext<CNCLibHub> hubcontext)
        {
            _hubcontext = hubcontext;
            _myhubcontext = _hubcontext;
        }

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

	    private async Task<SerialPortWrapper> GetPort(int id)
	    {
	        return await Task.FromResult(SerialPortList.GetPort(id));
	    }

	    #region Query/Info

        [HttpGet]
	    public async Task<IEnumerable<SerialPortDefinition>> Get()
        {
            return await Task.FromResult(SerialPortList.Ports.Select((e) => GetDefinition(e)));
        }

	    [HttpGet("{id:int}")]
	    public async Task<ActionResult<SerialPortDefinition>> Get(int id)
	    {
	        var port = await GetPort(id);
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
	    public async Task<ActionResult<SerialPortDefinition>> Connect(int id, int? baudrate=null, bool? dtrIsReset = true, bool? resetOnConnect=false)
        {
            bool dtrIsResetN0 = dtrIsReset ?? true;
            bool resetOnConnectN0 = resetOnConnect ?? false;
            int baudrateN0 = baudrate ?? 250000;

            var port = await GetPort(id);
	        if (port == null)
	        {
	            return NotFound();
	        }

	        if (port.IsConnected)
	        {
	            if (port.Serial.BaudRate == baudrateN0 && resetOnConnectN0 == false)
	            {
	                return Ok(GetDefinition(port));
	            }
	            await port.Serial.DisconnectAsync();
	        }

	        port.Serial.BaudRate = baudrateN0;
	        port.Serial.DtrIsReset = dtrIsResetN0;
            port.Serial.ResetOnConnect = resetOnConnectN0;

            await port.Serial.ConnectAsync(port.PortName);

	        await _hubcontext.Clients.All.SendAsync("connected", id);

            return Ok(GetDefinition(port));
	    }

	    [HttpPost("{id:int}/disconnect")]
	    public async Task<ActionResult<SerialPortDefinition>> DisConnect(int id)
	    {
	        var port = await GetPort(id);
	        if (port == null)
	        {
	            return NotFound();
	        }

            await port.Serial.DisconnectAsync();
	        port.Serial = null;

	        await _hubcontext.Clients.All.SendAsync("disconnected",id);

            return Ok();
	    }

        #endregion

        #region Send/Queue/Commands

        [HttpPost("{id:int}/queue")]
	    public async Task<ActionResult<IEnumerable<Framework.Arduino.SerialCommunication.SerialCommand>>> QueueCommand(int id, [FromBody] SerialCommands commands)
	    {
	        var port = await GetPort(id);

            if (port == null || commands == null || commands.Commands == null)
            {
                return NotFound();
            }

            var ret = await port.Serial.QueueCommandsAsync(commands.Commands);
            return Ok(ret);
	    }

	    [HttpPost("{id:int}/send")]
	    public async Task<ActionResult<IEnumerable<Framework.Arduino.SerialCommunication.SerialCommand>>> SendCommand(int id, [FromBody] SerialCommands commands)
	    {
	        var port = await GetPort(id);

	        if (port == null || commands==null || commands.Commands==null)
	        {
	            return NotFound();
	        }

            var ret = await port.Serial.SendCommandsAsync(commands.Commands, commands.TimeOut);
            return Ok(ret);
        }

        [HttpPost("{id:int}/sendWhileOk")]
        public async Task<ActionResult<IEnumerable<Framework.Arduino.SerialCommunication.SerialCommand>>> SendWhileOkCommand(int id, [FromBody] SerialCommands commands)
        {
            var port = await GetPort(id);

            if (port == null || commands == null || commands.Commands == null)
            {
                return NotFound();
            }

            var ret = new List<SerialCommand>();
            foreach(var c in commands.Commands)
            {
                var result = await port.Serial.SendCommandsAsync(new string[] { c }, commands.TimeOut);
                ret.AddRange(result);
                if (result.Count() > 0 && result.LastOrDefault().ReplyType != Framework.Arduino.SerialCommunication.EReplyType.ReplyOK)
                {
                   break;
                }
            }
            return Ok(ret);
        }

        [HttpPost("{id:int}/abort")]
	    public async Task<IActionResult> AbortCommand(int id)
	    {
	        var port = await GetPort(id);
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
	        var port = await GetPort(id);
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
	        var port = await GetPort(id);
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
	        var port = await GetPort(id);
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
	        var port = await GetPort(id);
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
	        var port = await GetPort(id);
	        if (port == null)
	        {
	            return NotFound();
	        }

	        port.Serial.ClearCommandHistory();
	        return Ok();
	    }

        [HttpGet("{id:int}/history")]
	    public async Task<ActionResult<IEnumerable<Framework.Arduino.SerialCommunication.SerialCommand>>> GetCommandHistory(int id)
	    {
	        var port = await GetPort(id);
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
