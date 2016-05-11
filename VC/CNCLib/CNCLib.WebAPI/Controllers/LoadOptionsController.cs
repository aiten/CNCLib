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

using CNCLib.GCode.Load;
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
	public class LoadOptionsController : ApiController
	{
		// GET api/values
		public IEnumerable<LoadInfo> Get()
		{
			using (var controller = Dependency.Resolve<IItemController>())
			{
				var list = new List<LoadInfo>();
				foreach (Item item in controller.GetAll(typeof(LoadInfo)))
				{
					list.Add((LoadInfo)controller.Create(item.ItemID));
				}
				return list;
			}
		}

		// GET api/values/5
		public LoadInfo Get(int id)
		{
			try
			{
				using (var controller = Dependency.Resolve<IItemController>())
				{
					object obj = controller.Create(id);
					if (obj != null || obj is LoadInfo)
					{
						return (LoadInfo)obj;
					}
					return null;
				}
			}
			catch (Exception e)
			{
				Request.CreateErrorResponse(HttpStatusCode.NotFound, e.Message);
				return null;
			}
		}

		// POST api/values
		public IHttpActionResult Post([FromBody]LoadInfo value)
		{
			try
			{
				if (!ModelState.IsValid || value == null)
				{
					return BadRequest(ModelState);
				}
				else
				{
					using (var controller = Dependency.Resolve<IItemController>())
					{
						int newid = controller.Add(value.SettingName,value);
						return CreatedAtRoute("DefaultApi", new
						{
							id = newid
						}, value);
					}
				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		// PUT api/values/5
		public IHttpActionResult Put(int id, [FromBody]LoadInfo value)
		{
			try
			{
				if (!ModelState.IsValid || value == null)
				{
					return BadRequest(ModelState);
				}
				else
				{
					using (var controller = Dependency.Resolve<IItemController>())
					{
						controller.Save(id,value.SettingName,value);
						return CreatedAtRoute("DefaultApi", new { id = id }, value);
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
			using (var controller = Dependency.Resolve<IItemController>())
			{
				var item = controller.Get(id);
				if (item == null)
				{
					Request.CreateErrorResponse(HttpStatusCode.NotFound, "id " + id + " not found");
				}
				else
				{
					controller.Delete(id);
				}
			}
		}
	}
}
