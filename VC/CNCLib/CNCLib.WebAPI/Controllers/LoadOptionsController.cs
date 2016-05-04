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
				if (value == null)
				{
					return BadRequest("Body object missing");
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
		public void Put(int id, [FromBody]LoadInfo value)
		{
			try
			{
				if (value == null)
				{
					Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Body object missing");
				}
				else
				{
					using (var controller = Dependency.Resolve<IItemController>())
					{
						controller.Save(id,value.SettingName,value);
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
