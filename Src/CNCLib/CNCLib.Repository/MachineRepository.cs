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
using CNCLib.Repository.Contracts.Entities;
using Framework.EF;
using Microsoft.EntityFrameworkCore;

namespace CNCLib.Repository
{
    public class MachineRepository : CRUDRepositoryBase<CNCLibContext, Machine,int>, IMachineRepository
	{
        public MachineRepository(CNCLibContext context) : base(context)
        {
        }

	    protected override IQueryable<Machine> AddInclude(IQueryable<Machine> query)
	    {
	        return query.Include(x => x.MachineCommands).Include(x => x.MachineInitCommands);
	    }

	    protected override IQueryable<Machine> AddPrimaryWhere(IQueryable<Machine> query, int key)
	    {
            return query.Where(m => m.MachineID == key);
	    }

        public async Task<IEnumerable<MachineCommand>> GetMachineCommands(int machineID)
		{
			return await Context.MachineCommands.
				Where(c => c.MachineID == machineID).
				ToListAsync();
		}

		public async Task<IEnumerable<MachineInitCommand>> GetMachineInitCommands(int machineID)
		{
			return await Context.MachineInitCommands.
				Where(c => c.MachineID == machineID).
				ToListAsync();
		}

        public async Task Store(Machine machine)
		{
			// search und update machine

			int id = machine.MachineID;
		    var machineCommands = machine.MachineCommands?.ToList() ?? new List<MachineCommand>();
		    var machineInitCommands = machine.MachineInitCommands?.ToList() ?? new List<MachineInitCommand>();

            var machineInDb = await Context.Machines.
				Where(m => m.MachineID == id).
				Include(d => d.MachineCommands).
				Include(d => d.MachineInitCommands).
				FirstOrDefaultAsync();

			if (machineInDb == default(Machine))
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

				Sync<MachineCommand>(
					machineInDb.MachineCommands, 
					machineCommands, 
					(x, y) => x.MachineCommandID > 0 && x.MachineCommandID == y.MachineCommandID);

				Sync<MachineInitCommand>(
					machineInDb.MachineInitCommands,
					machineInitCommands,
					(x, y) => x.MachineInitCommandID > 0 && x.MachineInitCommandID == y.MachineInitCommandID);

			}
		}
	}
}
