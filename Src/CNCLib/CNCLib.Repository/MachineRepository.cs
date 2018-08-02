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
using System.Threading.Tasks;
using CNCLib.Repository.Context;
using CNCLib.Repository.Contracts;
using Framework.EF;
using Microsoft.EntityFrameworkCore;

namespace CNCLib.Repository
{
    public class MachineRepository : CUDRepositoryBase<CNCLibContext, Contracts.Entities.Machine,int>, IMachineRepository
	{
        public MachineRepository(CNCLibContext context) : base(context)
        {
            IsPrimary = (m, id) => m.MachineID == id;
            AddInclude = i => i.Include(x => x.MachineCommands).Include(x => x.MachineInitCommands);
        }

        public async Task<IEnumerable<Contracts.Entities.MachineCommand>> GetMachineCommands(int machineID)
		{
			return await Context.MachineCommands.
				Where(c => c.MachineID == machineID).
				ToArrayAsync();
		}

		public async Task<IEnumerable<Contracts.Entities.MachineInitCommand>> GetMachineInitCommands(int machineID)
		{
			return await Context.MachineInitCommands.
				Where(c => c.MachineID == machineID).
				ToArrayAsync();
		}

		public async Task Store(Contracts.Entities.Machine machine)
		{
			// search und update machine

			int id = machine.MachineID;
		    var machineCommands = machine.MachineCommands?.ToList() ?? new List<Contracts.Entities.MachineCommand>();
		    var machineInitCommands = machine.MachineInitCommands?.ToList() ?? new List<Contracts.Entities.MachineInitCommand>();

            var machineInDb = await Context.Machines.
				Where(m => m.MachineID == id).
				Include(d => d.MachineCommands).
				Include(d => d.MachineInitCommands).
				FirstOrDefaultAsync();

			if (machineInDb == default(Contracts.Entities.Machine))
			{
				// add new

				AddEntity(machine);
                foreach (var mc in machineCommands)
			        AddEntity(mc);
			    foreach (var mic in machineInitCommands)
			        AddEntity(mic);
			}
            else
			{
				// syn with existing

				SetValue(machineInDb,machine);

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
	}
}
