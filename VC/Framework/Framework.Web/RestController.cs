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

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using Framework.Tools.Dependency;

namespace Framework.Web
{
	public abstract class RestController<T> : ApiController
	{
		public IEnumerable<T> Get()
		{
			using (var controller = Dependency.Resolve<IRest<T>>())
			{
				return controller.Get();
			}
		}

		// GET api/values/5
		//[ResponseType(T)]
		public IHttpActionResult Get(int id)
		{
			using (var controller = Dependency.Resolve<IRest<T>>())
			{
				var m = controller.Get(id);
				if (m == null)
				{
					return NotFound();
				}
				return Ok(m);
			}
		}

		// POST api/values == Create
		//[ResponseType(typeof(T))]
		public IHttpActionResult Post([FromBody]T value)
		{
			if (!ModelState.IsValid || value == null)
			{
				return BadRequest(ModelState);
			}
			try
			{
				using (var controller = Dependency.Resolve<IRest<T>>())
				{
					int newid = controller.Add(value);
					return CreatedAtRoute("DefaultApi", new { id = newid }, value);
				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// PUT api/values/5
		[ResponseType(typeof(void))]
		public IHttpActionResult Put(int id, [FromBody]T value)
		{
			if (!ModelState.IsValid || value == null)
			{
				return BadRequest(ModelState);
			}

			try
			{
				using (var controller = Dependency.Resolve<IRest<T>>())
				{
					if (controller.CompareId(id,value) == false)
					{
						return BadRequest("Missmatch between id and machineID");
					}

					controller.Update(id, value);
					return StatusCode(HttpStatusCode.NoContent);
				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// DELETE api/values/5
		//[ResponseType(typeof(T))]
		public IHttpActionResult Delete(int id)
		{
			using (var controller = Dependency.Resolve<IRest<T>>())
			{
				var value = controller.Get(id);
				if (value == null)
				{
					return NotFound();
				}

				controller.Delete(id, value);
				return Ok(value);
			}
		}
	}
}
