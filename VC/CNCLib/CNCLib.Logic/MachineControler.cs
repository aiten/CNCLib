////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

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
using System.Linq;
using System.Text;
using Framework.Tools;
using Framework.Logic;
using CNCLib.Repository.Contracts;
using CNCLib.Logic.Converter;
using CNCLib.Logic.Contracts;
using CNCLib.Logic.Contracts.DTO;
using Framework.Tools.Dependency;

namespace CNCLib.Logic
{
    public class MachineControler : ControlerBase, IMachineControler
	{
		public IEnumerable<Machine> GetMachines()
		{
			using (var rep = Dependency.Resolve<IMachineRepository>())
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

        public Machine GetMachine(int id)
        {
			using (var rep = Dependency.Resolve<IMachineRepository>())
			{
				var machine = rep.GetMachine(id);
				if (machine == null)
					return null;

				var dto = machine.Convert();
				return dto;
			}
        }
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

		public void Delete(Machine m)
        {
			using (var rep = Dependency.Resolve<IMachineRepository>())
			{
				rep.Delete(m.NewCloneProperties<Repository.Contracts.Entities.Machine, Machine>());
			}
        }

		public int StoreMachine(Machine m)
		{
			using (var rep = Dependency.Resolve<IMachineRepository>())
			{
				var me = m.Convert();
				return rep.Store(me);
			}
		}

		#region Default machine

		public int GetDetaultMachine()
		{
			using (var rep = Dependency.Resolve<IConfigurationRepository>())
			{
				var config = rep.Get("Environment", "DefaultMachineID");

				if (config == default(Repository.Contracts.Entities.Configuration))
					return -1;

				return int.Parse(config.Value);
			}
		}
		public void SetDetaultMachine(int defaultMachineID)
		{
			using (var rep = Dependency.Resolve<IConfigurationRepository>())
			{
				rep.Save(new Repository.Contracts.Entities.Configuration() { Group = "Environment", Name = "DefaultMachineID", Type = "Int32", Value = defaultMachineID.ToString() });
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
		// ~MachineControler() {
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
