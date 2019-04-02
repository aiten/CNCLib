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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CNCLib.Repository.Abstraction;
using CNCLib.Repository.Abstraction.Entities;
using CNCLib.Repository.Context;
using CNCLib.Shared;

using Framework.Repository;

using Microsoft.EntityFrameworkCore;

namespace CNCLib.Repository
{
    public class ItemRepository : CRUDRepository<CNCLibContext, Item, int>, IItemRepository
    {
        private readonly ICNCLibUserContext _userContext;

        #region ctr/default/overrides

        public ItemRepository(CNCLibContext context, ICNCLibUserContext userContext) : base(context)
        {
            _userContext = userContext ?? throw new ArgumentNullException();
        }

        protected override FilterBuilder<Item, int> FilterBuilder =>
            new FilterBuilder<Item, int>()
            {
                PrimaryWhere   = (query, key) => query.Where(item => item.ItemId == key),
                PrimaryWhereIn = (query, keys) => query.Where(item => keys.Contains(item.ItemId))
            };

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

        protected override void AssignValuesGraph(Item trackingEntity, Item values)
        {
            base.AssignValuesGraph(trackingEntity, values);
            Sync(trackingEntity.ItemProperties, values.ItemProperties, (x, y) => x.ItemId > 0 && x.ItemId == y.ItemId && x.Name == y.Name);
        }

        #endregion

        #region extra Queries

        public async Task<IList<Item>> Get(string typeIdString)
        {
            return await QueryWithOptional.Where(m => m.ClassName == typeIdString).Include(d => d.ItemProperties).ToListAsync();
        }

        #endregion
    }
}