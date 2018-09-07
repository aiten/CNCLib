﻿////////////////////////////////////////////////////////
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
using Framework.EF;
using Microsoft.EntityFrameworkCore;

namespace CNCLib.Repository
{
    public class ItemRepository : CRUDRepositoryBase<CNCLibContext, Item, int>, IItemRepository
	{
	    private ICNCLibUserContext _userContext;

	    public ItemRepository(CNCLibContext context, ICNCLibUserContext userContext) : base(context)
        {
            _userContext = userContext ?? throw new ArgumentNullException();
        }

        protected override IQueryable<Item> AddInclude(IQueryable<Item> query)
	    {
	        return query.Include(x => x.ItemProperties);
	    }

	    protected override IQueryable<Item> AddOptionalWhere(IQueryable<Item> query)
	    {
	        if (_userContext.UserID.HasValue)
	        {
	            return query.Where(x => x.UserID.HasValue == false || x.UserID.Value == _userContext.UserID.Value);
	        }

	        return base.AddOptionalWhere(query);
	    }

        protected override IQueryable<Item> AddPrimaryWhere(IQueryable<Item> query, int key)
	    {
	        return query.Where(m => m.ItemID == key);
	    }
	    protected override IQueryable<Item> AddPrimaryWhereIn(IQueryable<Item> query, IEnumerable<int> key)
	    {
	        return query.Where(m => key.Contains(m.ItemID));
	    }

        #region CRUD
        #endregion

        public async Task<IEnumerable<Item>> Get(string typeidstring)
	    {
	        return await QueryWithOptional.
	            Where(m => m.ClassName == typeidstring).
	            Include(d => d.ItemProperties).
	            ToListAsync();
	    }

	    protected override void AssignValuesGraph(Item trackingentity, Item values)
	    {
	        base.AssignValuesGraph(trackingentity, values);
	        Sync<ItemProperty>(
	            trackingentity.ItemProperties,
	           values.ItemProperties,
	            (x, y) => x.ItemID > 0 && x.ItemID == y.ItemID && x.Name == y.Name);
        }

/*
        public async Task Store(Item item)
		{
			// search und update item / itemproperties

			int id = item.ItemID;
		    var optValues = item.ItemProperties?.ToList() ?? new List<ItemProperty>();

			var itemInDb = await Context.Items.
				Where(m => m.ItemID == id).
				Include(d => d.ItemProperties).
                FirstOrDefaultAsync();

			if (itemInDb == default(Item))
			{
                // add new
				AddEntity(item);
			    foreach (var iv in optValues)
			        AddEntity(iv);

            }
            else
			{
                // syn with existing

                SetValue(itemInDb,item);

				// search und itemProperties (add and delete)

				Sync<ItemProperty>(
                    itemInDb.ItemProperties,
                    optValues, 
					(x, y) => x.ItemID > 0 && x.ItemID == y.ItemID && x.Name == y.Name);
			}
		}
*/
	}
}
