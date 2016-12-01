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

using System.Linq;
using CNCLib.Wpf.Models;
using Framework.Tools.Dependency;
using CNCLib.Logic.Contracts;
using CNCLib.ServiceProxy;
using CNCLib.Logic.Client;
using System.Threading.Tasks;
using System;

namespace CNCLib.Wpf.Helpers
{
	public class JoystickHelper
	{
		internal static async Task<Tuple<Joystick,int>> Load()
		{
			using (var controller = Dependency.Resolve<IDynItemController>())
			{
				var joystick = await controller.GetAll(typeof(Models.Joystick));
				if (joystick != null && joystick.Count() > 0)
				{
					int id = joystick.First().ItemID;
					return new Tuple<Joystick, int> ((Models.Joystick) await controller.Create(id),id);
				}
			}

			return new Tuple<Joystick, int> (new Joystick() { BaudRate = 250000, ComPort = @"com7" },-1);
		}

		internal static async Task<int> Save(Joystick joystick, int id)
		{
			using (var controller = Dependency.Resolve<IDynItemController>())
			{
				if (id >= 0)
				{
					await controller.Save(id, "Joystick", joystick);
					return id;
				}
				else
				{
					return await controller.Add("Joystick", joystick);
				}
			}
		}
	}
}
