/*
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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CNCLib.Repository.Abstraction;
using CNCLib.Repository.Abstraction.Entities;
using CNCLib.Repository.Context;

using Framework.Repository;

using Microsoft.EntityFrameworkCore;

namespace CNCLib.Repository
{
    public class MachineRepository : CrudRepository<CNCLibContext, Machine, int>, IMachineRepository
    {
        #region ctr/default/overrides

        public MachineRepository(CNCLibContext context) : base(context)
        {
        }

        protected override FilterBuilder<Machine, int> FilterBuilder =>
            new FilterBuilder<Machine, int>()
            {
                PrimaryWhere   = (query, key) => query.Where(item => item.MachineId == key),
                PrimaryWhereIn = (query, keys) => query.Where(item => keys.Contains(item.MachineId))
            };

        protected override IQueryable<Machine> AddInclude(IQueryable<Machine> query)
        {
            return query.Include(x => x.MachineCommands).Include(x => x.MachineInitCommands).Include(x => x.User);
        }

        protected override void AssignValuesGraph(Machine trackingEntity, Machine values)
        {
            base.AssignValuesGraph(trackingEntity, values);
            Sync(trackingEntity.MachineCommands,
                values.MachineCommands,
                (x, y) => x.MachineCommandId > 0 && x.MachineCommandId == y.MachineCommandId,
                x => x.Machine = null);
            Sync(trackingEntity.MachineInitCommands,
                values.MachineInitCommands,
                (x, y) => x.MachineInitCommandId > 0 && x.MachineInitCommandId == y.MachineInitCommandId,
                x => x.Machine = null);
        }

        #endregion

        #region extra Queries

        public async Task<IList<Machine>> GetByUser(int userId)
        {
            return await AddOptionalWhere(Query).Where(m => m.UserId == userId).ToListAsync();
        }

        public async Task<IList<MachineCommand>> GetMachineCommands(int machineId)
        {
            return await Context.Set<MachineCommand>().Where(c => c.MachineId == machineId).ToListAsync();
        }

        public async Task<IList<MachineInitCommand>> GetMachineInitCommands(int machineId)
        {
            return await Context.Set<MachineInitCommand>().Where(c => c.MachineId == machineId).ToListAsync();
        }

        #endregion
    }
}