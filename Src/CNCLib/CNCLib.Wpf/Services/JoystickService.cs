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

using System.Linq;

using CNCLib.Wpf.Models;

using CNCLib.Logic.Client;

using System.Threading.Tasks;
using System;

using Framework.Dependency;

namespace CNCLib.Wpf.Services
{
    public class JoystickService : IJoystickService
    {
        private readonly IDynItemController _dynService;

        public JoystickService(IDynItemController dynService)
        {
            _dynService = dynService;
        }

        public async Task<Tuple<Joystick, int>> Load()
        {
            var joystick = await _dynService.GetAll(typeof(Joystick));
            if (joystick != null && joystick.Any())
            {
                int id = joystick.First().ItemId;
                return new Tuple<Joystick, int>((Joystick)await _dynService.Create(id), id);
            }

            return new Tuple<Joystick, int>(new Joystick { BaudRate = 250000, ComPort = @"com7" }, -1);
        }

        public async Task<int> Save(Joystick joystick, int id)
        {
            if (id >= 0)
            {
                await _dynService.Save(id, "Joystick", joystick);
                return id;
            }

            return await _dynService.Add("Joystick", joystick);
        }
    }
}