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

using AutoMapper;

namespace CNCLib.Logic
{
	public sealed class LogicAutoMapperProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<CNCLib.Repository.Contracts.Entities.Machine, CNCLib.Logic.Contracts.DTO.Machine>().ReverseMap();
			CreateMap<CNCLib.Repository.Contracts.Entities.MachineInitCommand, CNCLib.Logic.Contracts.DTO.MachineInitCommand>().ReverseMap();
			CreateMap<CNCLib.Repository.Contracts.Entities.MachineCommand, CNCLib.Logic.Contracts.DTO.MachineCommand>().ReverseMap();

			CreateMap<CNCLib.Repository.Contracts.Entities.Item, CNCLib.Logic.Contracts.DTO.Item>();
		}
	}
}
