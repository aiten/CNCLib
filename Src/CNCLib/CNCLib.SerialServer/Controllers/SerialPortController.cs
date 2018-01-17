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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO.Ports;
using System.Threading.Tasks;
using CNCLib.SerialServer.SerialPort;
using Microsoft.AspNetCore.Mvc;

namespace CNCLib.SerialServer.Controllers
{
    [Route("api/[controller]")]
    public class SerialPortController : Controller
	{
        public SerialPortController() : base()
        {
        }
	    protected string CurrentUri => $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";

        public class SerialPortDefinition
	    {
	        public int Id { get; set; }
	        public string PortName { get; set; }
	    }

        [HttpGet]
	    public async Task<IEnumerable<SerialPortDefinition>> Get()
        {
            return SerialPortHelper.Ports.Select((e) => new SerialPortDefinition() { Id = e.Id, PortName = e.PortName});
        }

	    // GET api/values/5
	    //[ResponseType(T)]
	    [HttpGet("{id:int}")]
	    public async Task<IActionResult> Get(int id)
	    {
	        var m = SerialPortHelper.Ports.FirstOrDefault((s) => s.Id == id);
	        if (m == null)
	        {
	            return NotFound();
	        }
	        return Ok(new SerialPortDefinition() {Id = m.Id, PortName = m.PortName});
	    }

	    [HttpGet("refresh")]
	    public async Task<IEnumerable<SerialPortDefinition>> Refresh()
	    {
            SerialPortHelper.Refresh();
            return await Get();
	    }

	    // GET api/values/5
	    //[ResponseType(T)]
	    [HttpGet("{id:int}/open")]
	    public async Task<IActionResult> Open(int id, int? baudrate=null,bool? resetOnConnect=true)
	    {
	        var m = SerialPortHelper.Ports.FirstOrDefault((s) => s.Id == id);
	        if (m == null)
	        {
	            return NotFound();
	        }

	        m.Serial.BaudRate = baudrate ?? 250000;
	        m.Serial.ResetOnConnect = resetOnConnect ?? true;

            m.Serial.Connect(m.PortName);

	        return Ok(new SerialPortDefinition() { Id = m.Id, PortName = m.PortName });
	    }

    }
}
