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

using CNCLib.Repository.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CNCLib.Repository;
using Framework.Tools;
using System.Data.Entity;
using Framework.EF;
using CNCLib.Repository.Contracts;
using Framework.Tools.Pattern;

namespace CNCLib.Repository
{
    public class MachineRepository : CNCLibRepository, IMachineRepository
	{
		public Contracts.Entities.Machine[] GetMachines()
		{
            return Context.Machines.
                Include((d) => d.MachineCommands).
                Include((d) => d.MachineInitCommands).
                ToArray();
		}

		public Contracts.Entities.Machine GetMachine(int id)
        {
			return Context.Machines.
				Where((m) => m.MachineID == id).
				Include((d) => d.MachineCommands).
				Include((d) => d.MachineInitCommands).
				FirstOrDefault();
        }

		public void Delete(Contracts.Entities.Machine m)
        {
			m.MachineCommands = null;
			m.MachineInitCommands = null;
			Uow.MarkDeleted(m);
			Uow.ExecuteSqlCommand("delete from MachineCommand where MachineID = " + m.MachineID);
			Uow.ExecuteSqlCommand("delete from MachineInitCommand where MachineID = " + m.MachineID);
        }

		public Contracts.Entities.MachineCommand[] GetMachineCommands(int machineID)
		{
			return Context.MachineCommands.
				Where(c => c.MachineID == machineID).
				ToArray();
		}

		public Contracts.Entities.MachineInitCommand[] GetMachineInitCommands(int machineID)
		{
			return Context.MachineInitCommands.
				Where(c => c.MachineID == machineID).
				ToArray();
		}

		public void Store(Contracts.Entities.Machine machine)
		{
			// search und update machine

			int id = machine.MachineID;

			var machineInDb = Context.Machines.
				Where((m) => m.MachineID == id).
				Include((d) => d.MachineCommands).
				Include((d) => d.MachineInitCommands).
				FirstOrDefault();
			var machineCommands = machine.MachineCommands ?? new List<Contracts.Entities.MachineCommand>();
			var machineInitCommands = machine.MachineInitCommands ?? new List<Contracts.Entities.MachineInitCommand>();

			if (machineInDb == default(Contracts.Entities.Machine))
			{
				// add new

				Uow.MarkNew(machine);
			}
			else
			{
				// syn with existing

				machineInDb.CopyValueTypeProperties(machine);

				// search und update machinecommands (add and delete)

				Sync<Contracts.Entities.MachineCommand>(
					machineInDb.MachineCommands, 
					machineCommands, 
					(x, y) => x.MachineCommandID > 0 && x.MachineCommandID == y.MachineCommandID);

				Sync<Contracts.Entities.MachineInitCommand>(
					machineInDb.MachineInitCommands,
					machineInitCommands,
					(x, y) => x.MachineInitCommandID > 0 && x.MachineInitCommandID == y.MachineInitCommandID);

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
		// ~MachineRepository() {
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
