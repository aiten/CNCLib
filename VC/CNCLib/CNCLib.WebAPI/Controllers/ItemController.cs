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
using Framework.Web;
using CNCLib.ServiceProxy;

namespace CNCLib.WebAPI.Controllers
{
	public class ItemController : RestController<Item>
	{
		public IHttpActionResult Get(string classname)
		{
			using (IItemService service = Dependency.Resolve<IItemService>())
			{
				var m = service.GetByClassName(classname);
				if (m == null)
				{
					return NotFound();
				}
				return Ok(m);
			}
		}
	}

	public class ItemRest : IRest<Item>
	{
		private IItemService _service = Dependency.Resolve<IItemService>();

		public IEnumerable<Item> Get()
		{
			return _service.GetAll();
		}

		public Item Get(int id)
		{
			return _service.Get(id);
		}

		public int Add(Item value)
		{
			return _service.Add(value);
		}

		public void Update(int id, Item value)
		{
			_service.Update(value);
		}

		public void Delete(int id, Item value)
		{
			_service.Delete(value);
		}

		public bool CompareId(int id, Item value)
		{
			return id == value.ItemID;
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					_service.Dispose();
					_service = null;
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~MachineRest() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion

	}
}
