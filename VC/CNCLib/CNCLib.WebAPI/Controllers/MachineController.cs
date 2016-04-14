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
			using (var controler = Dependency.Resolve<IMachineControler>())
			{
				return controler.GetMachines();
			}
		}

		// GET api/values/5
		public Machine Get(int id)
		{
			using (var controler = Dependency.Resolve<IMachineControler>())
			{
				return controler.GetMachine(id);
			}
		}

		// POST api/values
		public void Post([FromBody]string value)
		{
		}

		// PUT api/values/5
		public void Put(int id, [FromBody]string value)
		{
		}

		// DELETE api/values/5
		public void Delete(int id)
		{
			using (var controler = Dependency.Resolve<IMachineControler>())
			{
				var machine = controler.GetMachine(id);
				if (machine != null)
					controler.Delete(machine);
			}
		}
	}
}
