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

using System.Collections.Generic;

using AutoMapper;

using CNCLib.Logic.Contract.DTO;

namespace CNCLib.Logic.Converter
{
    internal static class Converter
    {
        public static Machine ToDto(this Repository.Contract.Entities.Machine from, IMapper mapper)
        {
            return mapper.Map<Machine>(from);
        }

        public static Repository.Contract.Entities.Machine ToEntity(this Machine from, IMapper mapper)
        {
            return mapper.Map<Repository.Contract.Entities.Machine>(from);
        }

        public static Item ToDto(this Repository.Contract.Entities.Item from, IMapper mapper)
        {
            return mapper.Map<Item>(from);
        }

        public static Repository.Contract.Entities.Item ToEntity(this Item from, IMapper mapper)
        {
            return mapper.Map<Repository.Contract.Entities.Item>(from);
        }

        public static IEnumerable<Item> ToDto(this IEnumerable<Repository.Contract.Entities.Item> items, IMapper mapper)
        {
            return mapper.Map<IEnumerable<Item>>(items);
        }
    }
}