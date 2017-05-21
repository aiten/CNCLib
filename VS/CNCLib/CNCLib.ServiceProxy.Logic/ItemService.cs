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
using CNCLib.Logic.Contracts.DTO;
using CNCLib.Logic.Contracts;
using Framework.Tools.Dependency;
using System.Threading.Tasks;
using Framework.Tools;

namespace CNCLib.ServiceProxy.Logic
{
    public class ItemService : DisposeWrapper, IItemService
	{
		private IItemController _controller = Dependency.Resolve<IItemController>();

		public async Task<int> Add(Item value)
		{
			return await _controller.Add(value);
		}

		public async Task Delete(Item value)
		{
			await _controller.Delete(value);
		}

		public async Task<Item> Get(int id)
		{
			return await _controller.Get(id);
		}

		public async Task<IEnumerable<Item>> GetAll()
		{
			return await _controller.GetAll();
		}
		public async Task<IEnumerable<Item>> GetByClassName(string classname)
		{
			return await _controller.GetByClassName(classname);
		}

		public async Task<int> Update(Item value)
		{
			return await _controller.Update(value);
		}

        #region IDisposable Support

        protected override void DisposeManaged()
        {
            //_controller.Dispose();
            _controller = null;
        }

        #endregion

    }
}
