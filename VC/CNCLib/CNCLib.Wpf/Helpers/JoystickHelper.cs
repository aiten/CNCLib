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

namespace CNCLib.Wpf.Helpers
{
	public class JoystickHelper
	{
		internal static Joystick Load(out int id)
		{
			using (var controller = Dependency.Resolve<IItemService>())
			{
				var joystick = controller.GetAll(typeof(Models.Joystick));
				if (joystick != null && joystick.Count() > 0)
				{
					id = joystick.First().ItemID;
					return (Models.Joystick)controller.Create(id);
				}
			}
			id = -1;

			return new Joystick() { BaudRate = 250000, ComPort = @"com7" };

		}

		internal static void Save(Joystick joystick, ref int id)
		{
			using (var controller = Dependency.Resolve<IItemService>())
			{
				if (id >= 0)
				{
					controller.Save(id, "Joystick", joystick);
				}
				else
				{
					id = controller.Add("Joystick", joystick);
				}
			}
		}
	}
}
