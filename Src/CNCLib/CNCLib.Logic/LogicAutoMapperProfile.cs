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

namespace CNCLib.Logic
{
    public sealed class LogicAutoMapperProfile : Profile
    {
        public LogicAutoMapperProfile()
        {
            CreateMap<Repository.Contracts.Entities.Machine, Contracts.DTO.Machine>().ReverseMap();
            CreateMap<Repository.Contracts.Entities.MachineInitCommand, Contracts.DTO.MachineInitCommand>()
                .ReverseMap();
            CreateMap<Repository.Contracts.Entities.MachineCommand, Contracts.DTO.MachineCommand>().ReverseMap();

            CreateMap<Repository.Contracts.Entities.Item, Contracts.DTO.Item>().ReverseMap();
            CreateMap<Repository.Contracts.Entities.ItemProperty, Contracts.DTO.ItemProperty>().ReverseMap();
        }
    }
}