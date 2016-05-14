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

namespace CNCLib.Logic
{
	public class LoadOptionsController : ControllerBase, ILoadOptionsController
	{
		public IEnumerable<LoadInfo> GetAll()
		{
			using (var controller = Dependency.Resolve<IItemController>())
			{
				var list = new List<LoadInfo>();
				foreach (Item item in controller.GetAll(typeof(LoadInfo)))
				{
					LoadInfo li = (LoadInfo)controller.Create(item.ItemID);
					li.Id = item.ItemID;
					list.Add(li);
				}
				return list;
			}
		}

		public LoadInfo Get(int id)
		{
			using (var controller = Dependency.Resolve<IItemController>())
			{
				object obj = controller.Create(id);
				if (obj != null || obj is LoadInfo)
				{
					LoadInfo li = (LoadInfo)obj;
					li.Id = id;
					return (LoadInfo)obj;
				}

				return null;
			}
		}

		public void Delete(LoadInfo m)
        {
			using (var controller = Dependency.Resolve<IItemController>())
			{
				controller.Delete(m.Id);
			}
        }

		public int Add(LoadInfo m)
		{
			using (var controller = Dependency.Resolve<IItemController>())
			{
				return controller.Add(m.SettingName, m);
			}
		}

		public int Update(LoadInfo m)
		{
			using (var controller = Dependency.Resolve<IItemController>())
			{
				controller.Save(m.Id, m.SettingName, m);
				return m.Id;
			}
		}

		#region Default machine

		public int GetDetaultMachine()
		{
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IConfigurationRepository>(uow))
			{
				var config = rep.Get("Environment", "DefaultMachineID");

				if (config == default(Repository.Contracts.Entities.Configuration))
					return -1;

				return int.Parse(config.Value);
			}
		}
		public void SetDetaultMachine(int defaultMachineID)
		{
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IConfigurationRepository>(uow))
            {
                rep.Save(new Repository.Contracts.Entities.Configuration() { Group = "Environment", Name = "DefaultMachineID", Type = "Int32", Value = defaultMachineID.ToString() });
                uow.Save();
            }
        }

		#endregion

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
