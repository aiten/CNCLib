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
using System.Threading.Tasks;
using Framework.Tools;
using Framework.Logic;
using CNCLib.Repository.Interfaces;
using CNCLib.Repository;
using CNCLib.Repository.Entities;

namespace CNCLib.Logic
{
    public class MachineControler : ControlerBase, Interfaces.IMachineControler
	{
		public DTO.Machine[] GetMachines()
		{
			using (var rep = RepositoryFactory.Create<IMachineRepository>())
			{
				var machines = rep.GetMachines();
				List<DTO.Machine> l = new List<DTO.Machine>();
				l.AddCloneProperties(machines);
				return l.ToArray();
			}
		}

        public DTO.Machine GetMachine(int id)
        {
			using (var rep = RepositoryFactory.Create<IMachineRepository>())
			{
				var machine = rep.GetMachine(id);
				return ObjectConverter.NewCloneProperties<DTO.Machine, Repository.Entities.Machine>(machine);
			}
        }

        public void Delete(DTO.Machine m)
        {
			using (var rep = RepositoryFactory.Create<IMachineRepository>())
			{
				rep.Delete(m.NewCloneProperties<Repository.Entities.Machine, DTO.Machine>());
			}
        }

		public DTO.MachineCommand[] GetMachineCommands(int machineID)
		{
			using (var rep = RepositoryFactory.Create<IMachineRepository>())
			{
				var machines = rep.GetMachineCommands(machineID);
				List<DTO.MachineCommand> l = new List<DTO.MachineCommand>();
				l.AddCloneProperties(machines);
				return l.ToArray();
			}
		}
		public DTO.MachineInitCommand[] GetMachineInitCommands(int machineID)
		{
			using (var rep = RepositoryFactory.Create<IMachineRepository>())
			{
				var machines = rep.GetMachineInitCommands(machineID).OrderBy((c) => c.SeqNo);
				List<DTO.MachineInitCommand> l = new List<DTO.MachineInitCommand>();
				l.AddCloneProperties(machines);
				return l.ToArray();
			}
		}

		public int StoreMachine(DTO.Machine m)
		{
			using (var rep = RepositoryFactory.Create<IMachineRepository>())
			{
				var me = m.NewCloneProperties<Repository.Entities.Machine, DTO.Machine>();
				me.MachineCommands = m.MachineCommands.ToArray().CloneProperties<Repository.Entities.MachineCommand, DTO.MachineCommand>();
				me.MachineInitCommands = m.MachineInitCommands.ToArray().CloneProperties<Repository.Entities.MachineInitCommand, DTO.MachineInitCommand>();

				return rep.StoreMachine(me);
			}
		}

		#region Default machine

		public int GetDetaultMachine()
		{
			using (var rep = RepositoryFactory.Create<IConfigurationRepository>())
			{
				var config = rep.Get("Environment", "DefaultMachineID");

				if (config == default(Repository.Entities.Configuration))
					return -1;

				return int.Parse(config.Value);
			}
		}
		public void SetDetaultMachine(int defaultMachineID)
		{
			using (var rep = RepositoryFactory.Create<IConfigurationRepository>())
			{
				rep.Save(new Repository.Entities.Configuration() { Group = "Environment", Name = "DefaultMachineID", Type = "Int32", Value = defaultMachineID.ToString() });
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
