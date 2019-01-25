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

using AutoMapper;

using Framework.Dependency;

using MachineDto = CNCLib.Logic.Contract.DTO.Machine;

namespace CNCLib.Wpf.Models
{
    static class Converter
    {
        public static MachineDto Convert(this Machine from, IMapper mapper)
        {
            return mapper.Map<MachineDto>(from);
        }

        public static Machine Convert(this MachineDto from, IMapper mapper)
        {
            var to  = mapper.Map<Machine>(from);

            // AutoMapper do not mapper readonly observable collections
            foreach (var m in from.MachineCommands)
            {
                to.MachineCommands.Add(mapper.Map<MachineCommand>(m));
            }

            foreach (var mi in from.MachineInitCommands)
            {
                to.MachineInitCommands.Add(mapper.Map<MachineInitCommand>(mi));
            }

            return to;
        }
    }
}