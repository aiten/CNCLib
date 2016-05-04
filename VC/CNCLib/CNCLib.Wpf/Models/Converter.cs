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
using Framework.Tools;

namespace CNCLib.Wpf.Models
{
	static class Converter
	{
		public static Logic.Contracts.DTO.Machine Convert(this Machine from)
		{
			var dto =	ObjectConverter.NewCloneProperties<Logic.Contracts.DTO.Machine, Machine>(from);
			dto.MachineCommands = from.MachineCommands.CloneAsList<CNCLib.Logic.Contracts.DTO.MachineCommand, Models.MachineCommand>();
			dto.MachineInitCommands = from.MachineInitCommands.CloneAsList<CNCLib.Logic.Contracts.DTO.MachineInitCommand, Models.MachineInitCommand>();
			return dto;
		}

		public static Machine Convert(this Logic.Contracts.DTO.Machine from)
		{
			var model = ObjectConverter.NewCloneProperties<Machine, Logic.Contracts.DTO.Machine>(from);
			model.MachineCommands.Count();  //force init
			model.MachineInitCommands.Count();

			model.MachineCommands.AddCloneProperties(from.MachineCommands);
			model.MachineInitCommands.AddCloneProperties(from.MachineInitCommands);
			return model;
		}
	}
}
