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
		public Machine Get(int id)
		{
			using (var controller = Dependency.Resolve<IMachineController>())
			{
				return controller.GetMachine(id);
			}
		}

		// POST api/values == Create
		public IHttpActionResult Post([FromBody]Machine value)
		{
			try
			{
				if (!ModelState.IsValid || value==null)
				{
					return BadRequest(ModelState);
				}
				else
				{
					using (var controller = Dependency.Resolve<IMachineController>())
					{
						int machineid = controller.AddMachine(value);
						return CreatedAtRoute("DefaultApi", new { id = machineid }, value);
					}
				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// PUT api/values/5
		public IHttpActionResult Put(int id, [FromBody]Machine value)
		{
			try
			{
				if (!ModelState.IsValid || value == null)
				{
					return BadRequest(ModelState);
				}
				else if (id != value.MachineID)
				{
					return BadRequest("Missmatch between id and machineID");
				}
				else
				{
					using (var controller = Dependency.Resolve<IMachineController>())
					{
						controller.StoreMachine(value);
						int machineid = value.MachineID;
						return CreatedAtRoute("DefaultApi", new { id = machineid }, value);
					}
				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// DELETE api/values/5
		public void Delete(int id)
		{
			using (var controller = Dependency.Resolve<IMachineController>())
			{
				var machine = controller.GetMachine(id);
				if (machine == null)
				{
					Request.CreateErrorResponse(HttpStatusCode.NotFound,"id " + id + " not found");
				}
				else
				{
					controller.Delete(machine);
				}
			}
		}
	}
}
