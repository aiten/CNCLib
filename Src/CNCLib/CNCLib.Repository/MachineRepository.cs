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

using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CNCLib.Repository.Contracts;
using System.Threading.Tasks;
using Framework.Tools.Pattern;

namespace CNCLib.Repository
{
    public class MachineRepository : CNCLibRepository, IMachineRepository
	{
        public MachineRepository(IUnitOfWork uow) : base(uow)
        {
        }

        public async Task<Contracts.Entities.Machine[]> GetMachines()
		{
            return await Context.Machines.
                Include(d => d.MachineCommands).
                Include(d => d.MachineInitCommands).
                ToArrayAsync();
		}

		public async Task<Contracts.Entities.Machine> GetMachine(int id)
        {
			return await Context.Machines.
				Where(m => m.MachineID == id).
				Include(d => d.MachineCommands).
				Include(d => d.MachineInitCommands).
				FirstOrDefaultAsync();
        }

		public async Task Delete(Contracts.Entities.Machine m)
        {
			m.MachineCommands = null;
			m.MachineInitCommands = null;
			Uow.MarkDeleted(m);
			await Task.FromResult(0);
			//Uow.ExecuteSqlCommand("delete from MachineCommand where MachineID = " + m.MachineID); => delete cascade
			//Uow.ExecuteSqlCommand("delete from MachineInitCommand where MachineID = " + m.MachineID); => delete cascade
		}

		public async Task<Contracts.Entities.MachineCommand[]> GetMachineCommands(int machineID)
		{
			return await Context.MachineCommands.
				Where(c => c.MachineID == machineID).
				ToArrayAsync();
		}

		public async Task<Contracts.Entities.MachineInitCommand[]> GetMachineInitCommands(int machineID)
		{
			return await Context.MachineInitCommands.
				Where(c => c.MachineID == machineID).
				ToArrayAsync();
		}

		public async Task Store(Contracts.Entities.Machine machine)
		{
			// search und update machine

			int id = machine.MachineID;

			var machineInDb = await Context.Machines.
				Where(m => m.MachineID == id).
				Include(d => d.MachineCommands).
				Include(d => d.MachineInitCommands).
				FirstOrDefaultAsync();
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

				Uow.SetValue(machineInDb,machine);

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
