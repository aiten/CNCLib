////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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
using System.Data.Entity;
using CNCLib.Repository.Contracts;
using System.Threading.Tasks;
using Framework.Tools.Pattern;

namespace CNCLib.Repository
{
    public class ItemRepository : CNCLibRepository, IItemRepository
	{
        public ItemRepository(IUnitOfWork uow) : base(uow)
        {
        }

        public async Task<Contracts.Entities.Item[]> Get()
		{
			return await Context.Items.
				Include(d => d.ItemProperties).
				ToArrayAsync();
		}

		public async Task<Contracts.Entities.Item[]> Get(string typeidstring)
		{
			return await Context.Items.
				Where(m => m.ClassName == typeidstring).
				Include(d => d.ItemProperties).
				ToArrayAsync();
		}

		public async Task<Contracts.Entities.Item> Get(int id)
        {
			return await Context.Items.
				Where(m => m.ItemID == id).
				Include(d => d.ItemProperties).
				FirstOrDefaultAsync();
        }

		public async Task Delete(Contracts.Entities.Item e)
        {
			e.ItemProperties = null;
			Uow.MarkDeleted(e);
			await Task.FromResult(0);
			// Uow.ExecuteSqlCommand("delete from ItemProperty where ItemID = " + e.ItemID); => delete cascade
		}

		public async Task Store(Contracts.Entities.Item item)
		{
			// search und update machine

			int id = item.ItemID;

			var itemInDb = await Context.Items.
				Where(m => m.ItemID == id).
				Include(d => d.ItemProperties).
				FirstOrDefaultAsync();

            var optValues = item.ItemProperties ?? new List<Contracts.Entities.ItemProperty>();

			if (itemInDb == default(Contracts.Entities.Item))
			{
                // add new
				Uow.MarkNew(item);
			}
			else
			{
                // syn with existing

                Uow.SetValue(itemInDb,item);

				// search und update machinecommands (add and delete)

				Sync<Contracts.Entities.ItemProperty>(
                    itemInDb.ItemProperties,
                    optValues, 
					(x, y) => x.ItemID > 0 && x.ItemID == y.ItemID && x.Name == y.Name);
			}
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~MachineRepository() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion
	}
}
