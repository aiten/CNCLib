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

namespace CNCLib.Logic
{
	public class MachineController : ControllerBase, IMachineController
	{
		public IEnumerable<Machine> GetAll()
		{
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
			{
				var machines = rep.GetMachines();
				List<Machine> l = new List<Machine>();
				foreach (var m in machines)
				{
					l.Add(m.Convert());
				}
				return l;
			}
		}

        public Machine Get(int id)
        {
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
            {
                var machine = rep.GetMachine(id);
				if (machine == null)
					return null;

				var dto = machine.Convert();
				return dto;
			}
        }

		public void Delete(Machine m)
        {
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
            {
                rep.Delete(m.Convert());
                uow.Save();
			}
        }

		public int Add(Machine m)
		{
			using (var uow = Dependency.Resolve<IUnitOfWork>())
			using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
			{
				var me = m.Convert();
				me.MachineID = 0;
				foreach (var mc in me.MachineInitCommands) mc.MachineID = 0;
				foreach (var mi in me.MachineInitCommands) mi.MachineID = 0;
				rep.Store(me);
				uow.Save();
				return me.MachineID;
			}
		}

		public int Update(Machine m)
		{
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IMachineRepository>(uow))
            {
                var me = m.Convert();
				rep.Store(me);
                uow.Save();
                return me.MachineID;
			}
		}

		#region Default machine

		public Machine DefaultMachine()
		{
			var dto = new Machine()
			{
				Name = "New",
				ComPort = "comX",
				Axis = 3,
				SizeX = 130m,
				SizeY = 45m,
				SizeZ = 81m,
				SizeA = 360m,
				SizeB = 360m,
				SizeC = 360m,
				BaudRate = 115200,
				BufferSize = 63,
				CommandToUpper = false,
				ProbeSizeZ = 25,
				ProbeDist = 10m,
				ProbeDistUp = 3m,
				ProbeFeed = 100m,
				SDSupport = true,
				Spindle = true,
				Coolant = true,
				Rotate = true,
				Laser = false,
				MachineCommands = new MachineCommand[0],
				MachineInitCommands = new MachineInitCommand[0]
			};
			return dto;
		}

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
