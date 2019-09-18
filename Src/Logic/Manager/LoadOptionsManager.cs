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
using System.Threading.Tasks;

using CNCLib.Logic.Abstraction;
using CNCLib.Logic.Abstraction.DTO;
using CNCLib.Logic.Client;

using Framework.Logic;

namespace CNCLib.Logic.Manager
{
    public class LoadOptionsManager : ManagerBase, ILoadOptionsManager
    {
        private readonly IDynItemController _dynItemController;

        public LoadOptionsManager(IDynItemController dynItemController)
        {
            _dynItemController = dynItemController ?? throw new ArgumentNullException();
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