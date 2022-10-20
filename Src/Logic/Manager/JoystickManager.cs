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

public class JoystickManager : ManagerBase, IJoystickManager
{
    private readonly IDynItemController _dynItemController;

    public JoystickManager(IDynItemController dynItemController)
    {
        _dynItemController = dynItemController;
    }

    public async Task<IEnumerable<Joystick>> GetAllAsync()
    {
        var list = new List<Joystick>();
        foreach (DynItem item in await _dynItemController.GetAllAsync(typeof(Joystick)))
        {
            var joystick = (Joystick)await _dynItemController.CreateAsync(item.ItemId);
            joystick.Id = item.ItemId;
            list.Add(joystick);
        }

        return list;
    }

    public async Task<Joystick> GetAsync(int id)
    {
        object obj = await _dynItemController.CreateAsync(id);
        if (obj != null)
        {
            var joystick = (Joystick)obj;
            joystick.Id = id;
            return joystick;
        }

        return null;
    }

    public async Task DeleteAsync(Joystick joystick)
    {
        await _dynItemController.DeleteAsync(joystick.Id);
    }

    public async Task DeleteAsync(int key)
    {
        await _dynItemController.DeleteAsync(key);
    }

    public async Task<int> AddAsync(Joystick joystick)
    {
        return await _dynItemController.AddAsync($"Joystick{joystick.Id}", joystick);
    }

    public async Task UpdateAsync(Joystick joystick)
    {
        await _dynItemController.SaveAsync(joystick.Id, $"Joystick{joystick.Id}", joystick);
    }

    public async Task<Joystick> DefaultAsync()
    {
        var joystick = new Joystick()
        {
            SerialServer         = "https://localhost:5000",
            SerialServerUser     = "Admin",
            SerialServerPassword = "Serial.Server",
            BaudRate             = 250000,
            ComPort              = "COM7"
        };
        return await Task.FromResult(joystick);
    }

    public Task<IEnumerable<int>> AddAsync(IEnumerable<Joystick> values)
    {
        throw new System.NotImplementedException();
    }

    public Task UpdateAsync(IEnumerable<Joystick> values)
    {
        throw new System.NotImplementedException();
    }

    public Task DeleteAsync(IEnumerable<Joystick> values)
    {
        throw new System.NotImplementedException();
    }

    public Task DeleteAsync(IEnumerable<int> keys)
    {
        throw new System.NotImplementedException();
    }

    public Task<IEnumerable<Joystick>> GetAsync(IEnumerable<int> key)
    {
        throw new System.NotImplementedException();
    }

    public Task PatchAsync(int key, JsonPatchDocument<Joystick> patch)
    {
        throw new System.NotImplementedException();
    }
}