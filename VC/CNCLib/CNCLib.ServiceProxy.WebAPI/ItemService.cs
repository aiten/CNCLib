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


using System;
using System.Collections.Generic;
using CNCLib.Logic.Contracts.DTO;
using CNCLib.Logic.Contracts;
using Framework.Tools.Dependency;

namespace CNCLib.ServiceProxy.WebAPI
{
	public class ItemService : IItemService
	{
		private IItemController _controller = Dependency.Resolve<IItemController>();

		public int Add(string name, object value)
		{
			return _controller.Add(name, value);
		}

		public void Delete(int id)
		{
			_controller.Delete(id);
		}

		public Item Get(int id)
		{
			return _controller.Get(id);
		}

		public IEnumerable<Item> GetAll()
		{
			return _controller.GetAll();
		}
		public IEnumerable<Item> GetAll(Type t)
		{
			return _controller.GetAll(t);
		}

		public void Save(int id, string name, object value)
		{
			_controller.Save(id,name,value);
		}
		public object Create(int id)
		{
			return _controller.Create(id);
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					_controller.Dispose();
					_controller = null;
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~MachineRest() {
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
