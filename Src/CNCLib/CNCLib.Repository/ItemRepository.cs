////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2019 Herbert Aitenbichler

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
using CNCLib.Repository.Contract;
using CNCLib.Repository.Contract.Entities;
using CNCLib.Shared;

using Framework.Repository;

using Microsoft.EntityFrameworkCore;

namespace CNCLib.Repository
{
    public class ItemRepository : CRUDRepository<CNCLibContext, Item, int>, IItemRepository
    {
        private readonly ICNCLibUserContext _userContext;

        public ItemRepository(CNCLibContext context, ICNCLibUserContext userContext) : base(context)
        {
            _userContext = userContext ?? throw new ArgumentNullException();
        }

        protected override IQueryable<Item> AddInclude(IQueryable<Item> query)
        {
            return query.Include(x => x.ItemProperties).Include(x => x.User);
        }

        protected override IQueryable<Item> AddOptionalWhere(IQueryable<Item> query)
        {
            if (_userContext.UserId.HasValue)
            {
                return query.Where(x => x.UserId.HasValue == false || x.UserId.Value == _userContext.UserId.Value);
            }

            return base.AddOptionalWhere(query);
        }

        protected override IQueryable<Item> AddPrimaryWhere(IQueryable<Item> query, int key)
        {
            return query.Where(m => m.ItemId == key);
        }

        protected override IQueryable<Item> AddPrimaryWhereIn(IQueryable<Item> query, IEnumerable<int> key)
        {
            return query.Where(m => key.Contains(m.ItemId));
        }

        #region CRUD

        #endregion

        public async Task<IList<Item>> Get(string typeIdString)
        {
            return await QueryWithOptional.Where(m => m.ClassName == typeIdString).Include(d => d.ItemProperties).ToListAsync();
        }

        protected override void AssignValuesGraph(Item trackingEntity, Item values)
        {
            base.AssignValuesGraph(trackingEntity, values);
            Sync(trackingEntity.ItemProperties, values.ItemProperties, (x, y) => x.ItemId > 0 && x.ItemId == y.ItemId && x.Name == y.Name);
        }
    }
}