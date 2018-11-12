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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CNCLib.Repository.Context;
using CNCLib.Repository.Contracts;
using CNCLib.Repository.Contracts.Entities;
using CNCLib.Shared;

using Framework.Repository;

using Microsoft.EntityFrameworkCore;

namespace CNCLib.Repository
{
    public class MachineRepository : CRUDRepositoryBase<CNCLibContext, Machine, int>, IMachineRepository
    {
        private readonly ICNCLibUserContext _userContext;

        public MachineRepository(CNCLibContext context, ICNCLibUserContext userContext) : base(context)
        {
            _userContext = userContext ?? throw new ArgumentNullException();
        }

        protected override IQueryable<Machine> AddInclude(IQueryable<Machine> query)
        {
            return query.Include(x => x.MachineCommands).Include(x => x.MachineInitCommands).Include(x => x.User);
        }

        protected override IQueryable<Machine> AddOptionalWhere(IQueryable<Machine> query)
        {
            if (_userContext.UserId.HasValue)
            {
                return query.Where(x => x.UserId.HasValue == false || x.UserId.Value == _userContext.UserId.Value);
            }

            return base.AddOptionalWhere(query);
        }

        protected override IQueryable<Machine> AddPrimaryWhere(IQueryable<Machine> query, int key)
        {
            return query.Where(m => m.MachineId == key);
        }

        protected override IQueryable<Machine> AddPrimaryWhereIn(IQueryable<Machine> query, IEnumerable<int> key)
        {
            return query.Where(m => key.Contains(m.MachineId));
        }

        protected override void AssignValuesGraph(Machine trackingEntity, Machine values)
        {
            base.AssignValuesGraph(trackingEntity, values);
            Sync(trackingEntity.MachineCommands,     values.MachineCommands,     (x, y) => x.MachineCommandId > 0 && x.MachineCommandId == y.MachineCommandId);
            Sync(trackingEntity.MachineInitCommands, values.MachineInitCommands, (x, y) => x.MachineInitCommandId > 0 && x.MachineInitCommandId == y.MachineInitCommandId);
        }

        public async Task<IEnumerable<MachineCommand>> GetMachineCommands(int machineId)
        {
            return await Context.MachineCommands.Where(c => c.MachineId == machineId).ToListAsync();
        }

        public async Task<IEnumerable<MachineInitCommand>> GetMachineInitCommands(int machineId)
        {
            return await Context.MachineInitCommands.Where(c => c.MachineId == machineId).ToListAsync();
        }
    }
}