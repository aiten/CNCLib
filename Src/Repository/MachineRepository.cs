﻿/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) Herbert Aitenbichler

  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

namespace CNCLib.Repository;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CNCLib.Repository.Abstraction;
using CNCLib.Repository.Abstraction.Entities;
using CNCLib.Repository.Context;

using Framework.Repository;

using Microsoft.EntityFrameworkCore;

public class MachineRepository : CrudRepository<CNCLibContext, MachineEntity, int>, IMachineRepository
{
    #region ctr/default/overrides

    public MachineRepository(CNCLibContext context) : base(context)
    {
    }

    protected override FilterBuilder<MachineEntity, int> FilterBuilder =>
        new()
        {
            PrimaryWhere   = (query, key) => query.Where(item => item.MachineId == key),
            PrimaryWhereIn = (query, keys) => query.Where(item => keys.Contains(item.MachineId))
        };

    protected override IQueryable<MachineEntity> AddInclude(IQueryable<MachineEntity> query, params string[] includeProperties)
    {
        return base.AddInclude(query, includeProperties).Include(x => x.MachineCommands).Include(x => x.MachineInitCommands).Include(x => x.User);
    }

    protected override async Task AssignValuesGraphAsync(MachineEntity trackingEntity, MachineEntity values)
    {
        await base.AssignValuesGraphAsync(trackingEntity, values);
        await SyncAsync(trackingEntity.MachineCommands!,
            values.MachineCommands!,
            (x, y) => x.MachineCommandId > 0 && x.MachineCommandId == y.MachineCommandId,
            x => x.Machine = null);
        await SyncAsync(trackingEntity.MachineInitCommands!,
            values.MachineInitCommands!,
            (x, y) => x.MachineInitCommandId > 0 && x.MachineInitCommandId == y.MachineInitCommandId,
            x => x.Machine = null);
    }

    #endregion

    #region extra Queries

    public async Task<IList<MachineEntity>> GetByUserAsync(int userId)
    {
        return await QueryWithInclude().Where(m => m.UserId == userId).ToListAsync();
    }

    public async Task<IList<int>> GetIdByUserAsync(int userId)
    {
        return await Query.Where(m => m.UserId == userId).Select(m => m.MachineId).ToListAsync();
    }

    public async Task DeleteByUserAsync(int userId)
    {
        var machines = await TrackingQueryWithInclude().Where(m => m.UserId == userId).ToListAsync();
        DeleteEntities(machines);
    }

    public async Task<IList<MachineCommandEntity>> GetMachineCommandsAsync(int machineId)
    {
        return await Context.Set<MachineCommandEntity>().Where(c => c.MachineId == machineId).ToListAsync();
    }

    public async Task<IList<MachineInitCommandEntity>> GetMachineInitCommandsAsync(int machineId)
    {
        return await Context.Set<MachineInitCommandEntity>().Where(c => c.MachineId == machineId).ToListAsync();
    }

    #endregion
}