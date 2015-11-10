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
using CNCLib.Repository.Interface;

namespace CNCLib.Repository
{
    public class MachineRepository : RepositoryBase, IMachineRepository
	{
		public Entities.Machine[] GetMachines()
		{
			using (IUnitOfWork uow = UnitOfWorkFactory.Create())
			{
				return uow.Query<Entities.Machine>().
					ToList().ToArray();
			}
		}

		public Entities.Machine GetMachine(int id)
        {
			using (IUnitOfWork uow = UnitOfWorkFactory.Create())
			{
				return uow.Query<Entities.Machine>().
					Where((m) => m.MachineID == id).
					Include((d) => d.MachineCommands).
					Include((d) => d.MachineInitCommands).
					FirstOrDefault();
			}
        }

		public void Delete(Entities.Machine m)
        {
			using (IUnitOfWork uow = UnitOfWorkFactory.Create())
			{
				try
				{
					uow.BeginTransaction();

					m.MachineCommands = null;
					m.MachineInitCommands = null;
					uow.MarkDeleted(m);
					uow.ExecuteSqlCommand("delete from MachineCommand where MachineID = " + m.MachineID);
					uow.ExecuteSqlCommand("delete from MachineInitCommand where MachineID = " + m.MachineID);
					uow.Save();

					uow.CommitTransaction();
				}
				catch (Exception)
				{
					uow.RollbackTransaction();
					throw;
				}
			}
        }

		public Entities.MachineCommand[] GetMachineCommands(int machineID)
		{
			using (IUnitOfWork uow = UnitOfWorkFactory.Create())
			{
				return uow.Query<CNCLib.Repository.Entities.MachineCommand>().
					Where(c => c.MachineID == machineID).
					ToList().ToArray();
			}
		}

		public Entities.MachineInitCommand[] GetMachineInitCommands(int machineID)
		{
			using (IUnitOfWork uow = UnitOfWorkFactory.Create())
			{
				return uow.Query<CNCLib.Repository.Entities.MachineInitCommand>().
					Where(c => c.MachineID == machineID).
					ToList().ToArray();
			}
		}

		public int StoreMachine(Entities.Machine machine)
		{
			// search und update machine

			using (IUnitOfWork uow = UnitOfWorkFactory.Create())
			{
				int id = machine.MachineID;

				try
				{
					uow.BeginTransaction();

					var machineInDb = uow.Query<Entities.Machine>().
						Where((m) => m.MachineID == id).
						Include((d) => d.MachineCommands).
						Include((d) => d.MachineInitCommands).
						FirstOrDefault();
					var machineCommands = machine.MachineCommands ?? new List<Entities.MachineCommand>();
					var machineInitCommands = machine.MachineInitCommands ?? new List<Entities.MachineInitCommand>();

					if (machineInDb == default(Entities.Machine))
					{
						// add new

						machineInDb = machine;
						uow.MarkNew(machineInDb);
						uow.Save();						// this will save the Commands as well
						id = machineInDb.MachineID;
					}
					else
					{
						// syn with existing

						machineInDb.CopyValueTypeProperties(machine);

						// search und update machinecommands (add and delete)

						Sync<Entities.MachineCommand>(uow, 
							machineInDb.MachineCommands, 
							machineCommands, 
							(x, y) => x.MachineCommandID > 0 && x.MachineCommandID == y.MachineCommandID);

						Sync<Entities.MachineInitCommand>(uow,
							machineInDb.MachineInitCommands,
							machineInitCommands,
							(x, y) => x.MachineInitCommandID > 0 && x.MachineInitCommandID == y.MachineInitCommandID);

						uow.Save();
					}

					uow.CommitTransaction();
				}
				catch (Exception /*ex */)
				{
					uow.RollbackTransaction();
					throw;
				}

				return id;
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
