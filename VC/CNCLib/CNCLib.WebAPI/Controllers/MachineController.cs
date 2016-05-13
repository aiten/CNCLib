////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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

using CNCLib.Logic.Contracts;
using CNCLib.Logic.Contracts.DTO;
using Framework.Tools.Dependency;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace CNCLib.WebAPI.Controllers
{
	public class MachineController : ApiController
	{
		// GET api/values
		public IEnumerable<Machine> Get()
		{
			using (var controller = Dependency.Resolve<IMachineController>())
			{
				return controller.GetMachines();
			}
		}

		// GET api/values/5
		[ResponseType(typeof(Machine))]
		public IHttpActionResult Get(int id)
		{
			using (var controller = Dependency.Resolve<IMachineController>())
			{
				var m = controller.GetMachine(id);
				if (m == null)
				{
					return NotFound();
				}
				return Ok(m);
			}
		}

		// POST api/values == Create
		public IHttpActionResult Post([FromBody]Machine value)
		{
			if (!ModelState.IsValid || value==null)
			{
				return BadRequest(ModelState);
			}
			try
			{
				using (var controller = Dependency.Resolve<IMachineController>())
				{
					int machineid = controller.AddMachine(value);
					return CreatedAtRoute("DefaultApi", new { id = machineid }, value);
				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// PUT api/values/5
		[ResponseType(typeof(void))]
		public IHttpActionResult Put(int id, [FromBody]Machine value)
		{
			if (!ModelState.IsValid || value == null)
			{
				return BadRequest(ModelState);
			}
			if (id != value.MachineID)
			{
				return BadRequest("Missmatch between id and machineID");
			}

			try
			{
				using (var controller = Dependency.Resolve<IMachineController>())
				{
					controller.StoreMachine(value);
					return StatusCode(HttpStatusCode.NoContent);
//						int machineid = value.MachineID;
//						return CreatedAtRoute("DefaultApi", new { id = machineid }, value);
				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// DELETE api/values/5
		[ResponseType(typeof(Machine))]
		public IHttpActionResult Delete(int id)
		{
			using (var controller = Dependency.Resolve<IMachineController>())
			{
				var machine = controller.GetMachine(id);
				if (machine == null)
				{
					return NotFound();
				}

				controller.Delete(machine);
				return Ok(machine);
			}
		}
	}
}
