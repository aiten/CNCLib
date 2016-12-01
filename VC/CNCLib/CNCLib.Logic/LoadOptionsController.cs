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
using Framework.Tools;
using Framework.Logic;
using CNCLib.Repository.Contracts;
using CNCLib.Logic.Converter;
using CNCLib.Logic.Contracts;
using CNCLib.Logic.Contracts.DTO;
using Framework.Tools.Dependency;
using Framework.Tools.Pattern;
using CNCLib.GCode.Load;
using CNCLib.Logic.Client;
using System.Threading.Tasks;

namespace CNCLib.Logic
{
	public class LoadOptionsController : ControllerBase, ILoadOptionsController
	{
		public async Task<IEnumerable<LoadOptions>> GetAll()
		{
			using (var controller = Dependency.Resolve<IDynItemController>())
			{
				var list = new List<LoadOptions>();
				foreach (DynItem item in await controller.GetAll(typeof(LoadOptions)))
				{
					LoadOptions li = (LoadOptions) await controller.Create(item.ItemID);
					li.Id = item.ItemID;
					list.Add(li);
				}
				return (IEnumerable<LoadOptions>) list;
			}
		}

		public async Task<LoadOptions> Get(int id)
		{
			using (var controller = Dependency.Resolve<IDynItemController>())
			{
				object obj = await controller.Create(id);
				if (obj != null || obj is LoadOptions)
				{
					LoadOptions li = (LoadOptions)obj;
					li.Id = id;
					return (LoadOptions)obj;
				}

				return null;
			}
		}

		public async Task Delete(LoadOptions m)
        {
			using (var controller = Dependency.Resolve<IDynItemController>())
			{
				await controller.Delete(m.Id);
			}
        }

		public async Task<int> Add(LoadOptions m)
		{
			using (var controller = Dependency.Resolve<IDynItemController>())
			{
				return await controller.Add(m.SettingName, m);
			}
		}

		public async Task<int> Update(LoadOptions m)
		{
			using (var controller = Dependency.Resolve<IDynItemController>())
			{
				await controller.Save(m.Id, m.SettingName, m);
				return m.Id;
			}
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~MachineController() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		void IDisposable.Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion
	}
}
