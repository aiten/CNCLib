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

namespace CNCLib.Service.Logic
{
    using System.Threading.Tasks;

    using CNCLib.Logic.Abstraction;
    using CNCLib.Logic.Abstraction.DTO;
    using CNCLib.Service.Abstraction;

    using Framework.Service.Logic;

    public class MachineService : CrudService<Machine, int>, IMachineService
    {
        readonly IMachineManager _manager;

        public MachineService(IMachineManager manager) : base(manager)
        {
            _manager = manager;
        }

        public async Task<Machine> Default()
        {
            return await _manager.Default();
        }

        public async Task<int> GetDefault()
        {
            return await _manager.GetDefault();
        }

        public async Task SetDefault(int machineId)
        {
            await _manager.SetDefault(machineId);
        }

        public async Task<string> TranslateJoystickMessage(int machineId, string joystickMessage)
        {
            return await _manager.TranslateJoystickMessage(machineId, joystickMessage);
        }

        public async Task<string> TranslateJoystickMessage(Machine machine, string joystickMessage)
        {
            return await Task.FromResult(_manager.TranslateJoystickMessage(machine, joystickMessage));
        }

        public async Task<Machine> UpdateFromEeprom(Machine machine, uint[] eepromValues)
        {
            return await Task.FromResult(_manager.UpdateFromEeprom(machine, eepromValues));
        }
    }
}