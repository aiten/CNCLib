////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2018 Herbert Aitenbichler

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
using System.Threading.Tasks;
using CNCLib.Logic.Contracts.DTO;
using CNCLib.ServiceProxy;
using Framework.Web;

namespace CNCLib.WebAPI.Controllers
{
    public class LoadOptionsController : RestController<LoadOptions>
	{
        public LoadOptionsController(IRest<LoadOptions> controller) : base(controller)
        {
        }
    }

	public class LoadInfoRest : IRest<LoadOptions>
	{
        public LoadInfoRest(ILoadOptionsService service)
        {
            _service = service ?? throw new ArgumentNullException();
        }

		readonly ILoadOptionsService _service;

		public async Task<IEnumerable<LoadOptions>> Get()
		{
			return await _service.GetAll();
		}

		public async Task<LoadOptions> Get(int id)
		{
			return await _service.Get(id);
		}

		public async Task<int> Add(LoadOptions value)
		{
			return await _service.Add(value);
		}

		public async Task Update(int id, LoadOptions value)
		{
			await _service.Update(value);
		}

		public async Task Delete(int id, LoadOptions value)
		{
			await _service.Delete(value);
		}

		public bool CompareId(int id, LoadOptions value)
		{
			return true;
		}

		#region IDisposable Support
		private bool _disposedValue; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				_disposedValue = true;
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
