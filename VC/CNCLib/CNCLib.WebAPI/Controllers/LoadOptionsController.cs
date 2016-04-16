using CNCLib.GCode.Load;
using CNCLib.Logic.Contracts;
using CNCLib.Logic.Contracts.DTO;
using CNCLib.Repository.Context;
using Framework.EF;
using Framework.Tools.Dependency;
using Framework.Tools.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CNCLib.WebAPI.Controllers
{
	public class LoadOptionsController : ApiController
	{
		// GET api/values
		public IEnumerable<object> Get()
		{
			using (var controller = Dependency.Resolve<IItemController>())
			{
				return controller.GetAll();
			}
		}

		// GET api/values/5
		public string Get(int id)
		{
			try
			{
				using (var controller = Dependency.Resolve<IMachineController>())
				{
					var x =  controller.GetMachine(id);
					return "x";
				}
			}
			catch (Exception e)
			{
				return e.Message + e.InnerException.ToString();
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
			using (var controller = Dependency.Resolve<IMachineController>())
			{
				var machine = controller.GetMachine(id);
				if (machine != null)
					controller.Delete(machine);
			}
		}
	}
}
