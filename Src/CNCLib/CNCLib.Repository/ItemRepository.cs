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
using Microsoft.EntityFrameworkCore;
using CNCLib.Repository.Contracts;
using System.Threading.Tasks;
using CNCLib.Repository.Context;
using Framework.Tools.Pattern;

namespace CNCLib.Repository
{
    public class ItemRepository : CNCLibRepository<Contracts.Entities.Item>, IItemRepository
	{
        public ItemRepository(CNCLibContext context) : base(context)
        {
        }

        #region CUD

        public async Task<IEnumerable<Contracts.Entities.Item>> GetAll()
		{
			return await Query.
				Include(d => d.ItemProperties).
				ToListAsync();
		}

	    public async Task<Contracts.Entities.Item> Get(int id)
	    {
	        return await Query.
	            Where(m => m.ItemID == id).
	            Include(d => d.ItemProperties).
	            FirstOrDefaultAsync();
	    }

	    public async Task<Contracts.Entities.Item> GetTracking(int id)
	    {
	        return await TrackingQuery.
	            Where(m => m.ItemID == id).
	            Include(d => d.ItemProperties).
	            FirstOrDefaultAsync();
	    }

        public async Task<IEnumerable<Contracts.Entities.Item>> Get(string typeidstring)
		{
			return await Query.
                Where(m => m.ClassName == typeidstring).
				Include(d => d.ItemProperties).
			    ToListAsync();
		}

	    public void Add(Contracts.Entities.Item e)
	    {
	        AddEntity(e);
	    }

        public void Delete(Contracts.Entities.Item e)
	    {
	        e.ItemProperties = null;
	        DeleteEntity(e);
	    }

        #endregion

	    public async Task Store(Contracts.Entities.Item item)
		{
			// search und update item / itemproperties

			int id = item.ItemID;
		    var optValues = item.ItemProperties?.ToList() ?? new List<Contracts.Entities.ItemProperty>();

			var itemInDb = await Context.Items.
				Where(m => m.ItemID == id).
				Include(d => d.ItemProperties).
                FirstOrDefaultAsync();

			if (itemInDb == default(Contracts.Entities.Item))
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

				Sync<Contracts.Entities.ItemProperty>(
                    itemInDb.ItemProperties,
                    optValues, 
					(x, y) => x.ItemID > 0 && x.ItemID == y.ItemID && x.Name == y.Name);
			}
		}
	}
}
