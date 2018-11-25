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

using CNCLib.Repository.Contract.Entities;

namespace CNCLib.Logic
{
    public sealed class LogicAutoMapperProfile : Profile
    {
        public LogicAutoMapperProfile()
        {
            CreateMap<Machine, Contract.DTO.Machine>().ReverseMap();
            CreateMap<MachineInitCommand, Contract.DTO.MachineInitCommand>().ReverseMap();
            CreateMap<MachineCommand, Contract.DTO.MachineCommand>().ReverseMap();

            CreateMap<Item, Contract.DTO.Item>().ReverseMap();
            CreateMap<ItemProperty, Contract.DTO.ItemProperty>().ReverseMap();
        }
    }
}