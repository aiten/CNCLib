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
using System.Threading.Tasks;

using CNCLib.Logic.Client;
using CNCLib.Logic.Contract;
using CNCLib.Logic.Contract.DTO;

using Framework.Logic;

namespace CNCLib.Logic.Manager
{
    public class LoadOptionsManager : ManagerBase, ILoadOptionsManager
    {
        private readonly IDynItemController _dynItemController;

        public LoadOptionsManager(IDynItemController dynItemController)
        {
            _dynItemController = dynItemController ?? throw new ArgumentException();
        }

        public async Task<IEnumerable<LoadOptions>> GetAll()
        {
            var list = new List<LoadOptions>();
            foreach (DynItem item in await _dynItemController.GetAll(typeof(LoadOptions)))
            {
                var li = (LoadOptions)await _dynItemController.Create(item.ItemId);
                li.Id = item.ItemId;
                list.Add(li);
            }

            return list;
        }

        public async Task<LoadOptions> Get(int id)
        {
            object obj = await _dynItemController.Create(id);
            if (obj != null)
            {
                var li = (LoadOptions)obj;
                li.Id = id;
                return (LoadOptions)obj;
            }

            return null;
        }

        public async Task Delete(LoadOptions m)
        {
            await _dynItemController.Delete(m.Id);
        }

        public async Task Delete(int key)
        {
            await _dynItemController.Delete(key);
        }

        public async Task<int> Add(LoadOptions m)
        {
            return await _dynItemController.Add(m.SettingName, m);
        }

        public async Task Update(LoadOptions m)
        {
            await _dynItemController.Save(m.Id, m.SettingName, m);
        }

        public Task<IEnumerable<int>> Add(IEnumerable<LoadOptions> values)
        {
            throw new System.NotImplementedException();
        }

        public Task Update(IEnumerable<LoadOptions> values)
        {
            throw new System.NotImplementedException();
        }

        public Task Delete(IEnumerable<LoadOptions> values)
        {
            throw new System.NotImplementedException();
        }

        public Task Delete(IEnumerable<int> keys)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<LoadOptions>> Get(IEnumerable<int> key)
        {
            throw new System.NotImplementedException();
        }

        #region IDisposable Support

        // see ManagerBase

        #endregion
    }
}