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
using System.Threading.Tasks;
using CNCLib.Logic.Contracts.DTO;
using CNCLib.ServiceProxy;
using Framework.Web;
using Microsoft.AspNetCore.Mvc;

namespace CNCLib.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class MachineController : RestController<Machine>
	{
        public MachineController(IRest<Machine> rest, IMachineService machineservice) : base(rest)
        {
            _machineservice = machineservice ?? throw new ArgumentNullException();
        }

        private IMachineService _machineservice;

	    #region default REST

	    [HttpGet("test/{id:int}")]
	    public async Task<ActionResult> Get(int id)
	    {
	        return await ControllerExtension.Get<Machine,int>(this,_machineservice, id);
	    }
/*
	    [HttpPost]
	    [Route("test/bulk")]
	    public async Task<ActionResult> Add([FromBody] IEnumerable<Machine> values)
	    {
	        return await ControllerExtension.Get<Machine, int>(this, _machineservice, values);
	    }
*/
	    [HttpPost]
	    [Route("test")]
	    public async Task<ActionResult> Add([FromBody] Machine value)
	    {
	        return await ControllerExtension.Add<Machine, int>(this, _machineservice, value);
	    }
/*
	    [HttpPut]
	    [Route("area/bulk")]
	    public async Task<IHttpActionResult> Update([FromBody] IEnumerable<Area> values)
	    {
	        return await Update(_manager, values);
	    }
*/
	    [HttpPut]
	    [Route("test/{id}")]
	    public async Task<ActionResult> Update(int id, [FromBody]Machine value)
	    {
	        return await ControllerExtension.Update<Machine, int>(this,_machineservice, id, value.MachineID, value);
	    }

	    [HttpDelete]
	    [Route("test/{id}")]
	    public async Task<ActionResult> Delete(int id)
	    {
	        return await ControllerExtension.Delete<Machine, int>(this,_machineservice, id);
	    }
/*
	    [HttpDelete]
	    [Route("area/bulk")]
	    public async Task<IHttpActionResult> Delete([FromUri] int[] id)
	    {
	        return await DeleteREST(_manager, id);
	    }
*/
	    #endregion

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Machine>>> Get()
        {
            return await this.GetAll<Machine>(Rest);
        }

        [Route("default")]
		[HttpGet]
		public async Task<IActionResult> DefaultMachine()
		{
			var m = await _machineservice.DefaultMachine();
			if (m == null)
			{
				return NotFound();
			}
			return Ok(m);
		}

		[Route("defaultmachine")]
		[HttpGet] //Always explicitly state the accepted HTTP method
		public async Task<IActionResult> GetDetaultMachine()
		{
			int id = await _machineservice.GetDetaultMachine();
			return Ok(id);
		}

		[Route("defaultmachine")]
		[HttpPut] //Always explicitly state the accepted HTTP method
		public async Task<IActionResult> SetDetaultMachine(int id)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				await _machineservice.SetDetaultMachine(id);
				return StatusCode(204);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
