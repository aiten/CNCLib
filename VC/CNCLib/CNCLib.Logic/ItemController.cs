////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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
using Framework.Logic;
using CNCLib.Repository.Contracts;
using CNCLib.Logic.Converter;
using CNCLib.Logic.Contracts;
using CNCLib.Logic.Contracts.DTO;
using Framework.Tools.Dependency;
using Framework.Tools.Pattern;
using System.Threading.Tasks;

namespace CNCLib.Logic
{
    public class ItemController : ControllerBase, IItemController
	{
		public async Task<IEnumerable<Item>> GetAll()
		{
			using (var uow = Dependency.Resolve<IUnitOfWork>())
			using (var rep = Dependency.ResolveRepository<IItemRepository>(uow))
			{
				return Convert(await rep.Get());
			}
		}

		public async Task<IEnumerable<Item>> GetByClassName(string classname)
		{
			using (var uow = Dependency.Resolve<IUnitOfWork>())
			using (var rep = Dependency.ResolveRepository<IItemRepository>(uow))
			{
				return Convert(await rep.Get(classname));
			}
		}

		public async Task<Item> Get(int id)
		{
			using (var uow = Dependency.Resolve<IUnitOfWork>())
			using (var rep = Dependency.ResolveRepository<IItemRepository>(uow))
			{
				return Convert(await rep.Get(id));
			}
		}


		public async Task Delete(Item item)
		{
			using (var uow = Dependency.Resolve<IUnitOfWork>())
			using (var rep = Dependency.ResolveRepository<IItemRepository>(uow))
			{
				await rep.Delete(item.Convert());
				await uow.Save();
			}
		}

		public async Task<int> Add(Item item)
		{
			using (var uow = Dependency.Resolve<IUnitOfWork>())
			using (var rep = Dependency.ResolveRepository<IItemRepository>(uow))
			{
				var me = item.Convert();
				me.ItemID = 0;
				foreach (var mc in me.ItemProperties) mc.ItemID = 0;
				await rep.Store(me);
				await uow.Save();
				return me.ItemID;
			}
		}

		public async Task<int> Update(Item item)
		{
			using (var uow = Dependency.Resolve<IUnitOfWork>())
			using (var rep = Dependency.ResolveRepository<IItemRepository>(uow))
			{
				var me = item.Convert();
				await rep.Store(me);
				await uow.Save();
				return me.ItemID;
			}
		}

		private static Item Convert(Repository.Contracts.Entities.Item item)
		{
			if (item == null)
				return null;

			var dto = item.Convert();
			return dto;
		}

		private static IEnumerable<Item> Convert(Repository.Contracts.Entities.Item[] items)
		{
			List<Item> l = new List<Item>();
			foreach (var i in items)
			{
				l.Add(i.Convert());
			}
			return l;
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
		// ~ItemController() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}

        #endregion
    }
}
