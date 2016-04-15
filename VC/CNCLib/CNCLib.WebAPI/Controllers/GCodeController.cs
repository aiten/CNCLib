using CNCLib.GCode.Commands;
using CNCLib.GCode.Load;
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
	public class GCodeController : ApiController
	{
		// GET api/values
		public IEnumerable<string> Get()
		{
			return new String[] { "todo" };
		}

		// GET api/values/5
		public IEnumerable<string> Get(int id)
		{
			LoadInfo opt = new LoadInfo { FileName = @"c:\data\heikes-mietzi.hpgl" };
			var load = new LoadHPGL() { LoadOptions = opt };
			load.Load();

			var list = new List<string>();

			Command last = null;

			foreach (var r in load.Commands)
			{
				string[] cmds = r.GetGCodeCommands(last != null ? last.CalculatedEndPosition : null);
				if (cmds != null)
				{
					foreach (String str in cmds)
					{
						list.Add(str);
					}
				}
				last = r;
			}
			return list;
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
		}
	}
}
