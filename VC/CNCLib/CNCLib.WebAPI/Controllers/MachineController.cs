using CNCLib.Logic.Contracts;
using CNCLib.Logic.Contracts.DTO;
using Framework.Tools.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
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
				if (value == null)
				{
					return BadRequest("Body object missing");
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
		public void Put(int id, [FromBody]Machine value)
		{
			try
			{
				if (value == null)
				{
					Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not read machine from body");
				}
				else
				{
					using (var controller = Dependency.Resolve<IMachineController>())
					{
						int machineid = controller.StoreMachine(value);
					}
				}
			}
			catch (Exception ex)
			{
				Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
			}
		}

		// DELETE api/values/5
		public void Delete(int id)
		{
			using (var controller = Dependency.Resolve<IMachineController>())
			{
				var machine = controller.GetMachine(id);
				if (machine != null)
					controller.Delete(machine);
			}
		}
	}
}
