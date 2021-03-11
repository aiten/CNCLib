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

namespace CNCLib.Repository
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CNCLib.Repository.Abstraction;
    using CNCLib.Repository.Abstraction.Entities;
    using CNCLib.Repository.Context;

    using Framework.Repository;

    using Microsoft.EntityFrameworkCore;

    public class ItemRepository : CrudRepository<CNCLibContext, ItemEntity, int>, IItemRepository
    {
        #region ctr/default/overrides

        public ItemRepository(CNCLibContext context) : base(context)
        {
        }

        protected override FilterBuilder<ItemEntity, int> FilterBuilder =>
            new()
            {
                PrimaryWhere   = (query, key) => query.Where(item => item.ItemId == key),
                PrimaryWhereIn = (query, keys) => query.Where(item => keys.Contains(item.ItemId))
            };

        protected override IQueryable<ItemEntity> AddInclude(IQueryable<ItemEntity> query)
        {
            return query.Include(x => x.ItemProperties).Include(x => x.User);
        }

        protected override void AssignValuesGraph(ItemEntity trackingEntity, ItemEntity values)
        {
            base.AssignValuesGraph(trackingEntity, values);
            Sync(trackingEntity.ItemProperties,
                values.ItemProperties,
                (x, y) => x.ItemId > 0 && x.ItemId == y.ItemId && x.Name == y.Name,
                x => x.Item = null);
        }

        #endregion

        #region extra Queries

        public async Task<IList<ItemEntity>> GetByUser(int userId)
        {
            return await QueryWithInclude.Where(m => m.UserId == userId).ToListAsync();
        }
        
        public async Task<IList<int>> GetIdByUser(int userId)
        {
            return await Query.Where(item => item.UserId == userId).Select(item => item.ItemId).ToListAsync();
        }

        public async Task DeleteByUser(int userId)
        {
            var items = await TrackingQueryWithInclude.Where(m => m.UserId == userId).ToListAsync();
            DeleteEntities(items);
        }

        public async Task<IList<ItemEntity>> Get(int userId, string typeIdString)
        {
            return await QueryWithInclude
                .Where(i => i.UserId == userId && i.ClassName == typeIdString)
                .Include(d => d.ItemProperties)
                .ToListAsync();
        }

        #endregion
    }
}