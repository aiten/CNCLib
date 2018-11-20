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

using MachineDto = CNCLib.Logic.Contract.DTO.Machine;
using MachineInitCommandDto = CNCLib.Logic.Contract.DTO.MachineInitCommand;
using MachineCommandDto = CNCLib.Logic.Contract.DTO.MachineCommand;

namespace CNCLib.Wpf
{
    public sealed class WpfAutoMapperProfile : Profile
    {
        public WpfAutoMapperProfile()
        {
            CreateMap<Models.Machine, MachineDto>().ReverseMap();
            CreateMap<Models.MachineInitCommand, MachineInitCommandDto>().ReverseMap();
            CreateMap<Models.MachineCommand, MachineCommandDto>().ReverseMap();
        }
    }
}