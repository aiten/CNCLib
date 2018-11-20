////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2018 Herbert Aitenbichler

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

using AutoMapper;

using Framework.Dependency;

using MachineDto = CNCLib.Logic.Contract.DTO.Machine;

namespace CNCLib.Wpf.Models
{
    static class Converter
    {
        public static MachineDto Convert(this Machine from)
        {
            var map = Dependency.Resolve<IMapper>();
            return map.Map<MachineDto>(from);
        }

        public static Machine Convert(this MachineDto from)
        {
            var map = Dependency.Resolve<IMapper>();
            var to  = map.Map<Machine>(from);

            // AutoMapper do not map readonly observable collections
            foreach (var m in from.MachineCommands)
            {
                to.MachineCommands.Add(map.Map<MachineCommand>(m));
            }

            foreach (var mi in from.MachineInitCommands)
            {
                to.MachineInitCommands.Add(map.Map<MachineInitCommand>(mi));
            }

            return to;
        }
    }
}