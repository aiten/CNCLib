﻿/*
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

namespace CNCLib.Repository;

using System.Collections.Generic;
using System.Threading.Tasks;

using CNCLib.Repository.Abstraction;
using CNCLib.Repository.Abstraction.Entities;
using CNCLib.Repository.Context;

using Framework.Repository;

public class InitRepository : RepositoryBase<CNCLibContext>, IInitRepository
{
    #region ctr/default/overrides

    public InitRepository(CNCLibContext context) : base(context)
    {
    }

    #endregion

    public async Task InitializeAsync(int userId)
    {
        new CNCLibDefaultData(Context).ImportForUser(userId);
        await Task.CompletedTask;
    }

    public async Task AddDefaultMachinesAsync(int userId)
    {
        new CNCLibDefaultData(Context).ImportForUserMachine(userId);
        await Task.CompletedTask;
    }

    public async Task AddDefaultItemsAsync(int userId)
    {
        new CNCLibDefaultData(Context).ImportForUserItem(userId);
        await Task.CompletedTask;
    }

    public async Task AddDefaultFilesAsync(int userId)
    {
        new CNCLibDefaultData(Context).ImportForUserFile(userId);
        await Task.CompletedTask;
    }

    public IList<MachineEntity> GetDefaultMachines()
    {
        return new CNCLibDefaultData(Context).GetDefaultMachines();
    }

    public IList<ItemEntity> GetDefaultItems()
    {
        return new CNCLibDefaultData(Context).GetDefaultItems();
    }

    public IList<UserFileEntity> GetDefaultFiles()
    {
        return new CNCLibDefaultData(Context).GetDefaultFiles();
    }
}