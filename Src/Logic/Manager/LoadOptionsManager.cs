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

namespace CNCLib.Logic.Manager;

using System.Collections.Generic;
using System.Threading.Tasks;

using CNCLib.Logic.Abstraction;
using CNCLib.Logic.Abstraction.DTO;
using CNCLib.Logic.Client;

using Framework.Logic;

using Microsoft.AspNetCore.JsonPatch;

public class LoadOptionsManager : ManagerBase, ILoadOptionsManager
{
    private readonly IDynItemController _dynItemController;

    public LoadOptionsManager(IDynItemController dynItemController)
    {
        _dynItemController = dynItemController;
    }

    public async Task<IEnumerable<LoadOptions>> GetAllAsync()
    {
        var list = new List<LoadOptions>();
        foreach (DynItem item in await _dynItemController.GetAllAsync(typeof(LoadOptions)))
        {
            var loadOption = (LoadOptions)await _dynItemController.CreateAsync(item.ItemId);
            loadOption.Id = item.ItemId;
            list.Add(loadOption);
        }

        return list;
    }

    public async Task<LoadOptions> GetAsync(int id)
    {
        object obj = await _dynItemController.CreateAsync(id);
        if (obj != null)
        {
            var loadOption = (LoadOptions)obj;
            loadOption.Id = id;
            return loadOption;
        }

        return null;
    }

    public async Task DeleteAsync(LoadOptions loadOption)
    {
        await _dynItemController.DeleteAsync(loadOption.Id);
    }

    public async Task DeleteAsync(int key)
    {
        await _dynItemController.DeleteAsync(key);
    }

    public async Task<int> AddAsync(LoadOptions loadOption)
    {
        return await _dynItemController.AddAsync(loadOption.SettingName, loadOption);
    }

    public async Task UpdateAsync(LoadOptions loadOption)
    {
        await _dynItemController.SaveAsync(loadOption.Id, loadOption.SettingName, loadOption);
    }

    public Task<IEnumerable<int>> AddAsync(IEnumerable<LoadOptions> values)
    {
        throw new System.NotImplementedException();
    }

    public Task UpdateAsync(IEnumerable<LoadOptions> values)
    {
        throw new System.NotImplementedException();
    }

    public Task DeleteAsync(IEnumerable<LoadOptions> values)
    {
        throw new System.NotImplementedException();
    }

    public Task DeleteAsync(IEnumerable<int> keys)
    {
        throw new System.NotImplementedException();
    }

    public Task<IEnumerable<LoadOptions>> GetAsync(IEnumerable<int> key)
    {
        throw new System.NotImplementedException();
    }

    public Task PatchAsync(int key, JsonPatchDocument<LoadOptions> patch)
    {
        throw new System.NotImplementedException();
    }
}