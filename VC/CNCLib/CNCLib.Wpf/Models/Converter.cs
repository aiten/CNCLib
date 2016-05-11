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
using AutoMapper;
using Framework.Tools;
using Framework.Tools.Dependency;

namespace CNCLib.Wpf.Models
{
	static class Converter
	{
		public static Logic.Contracts.DTO.Machine Convert(this Machine from)
		{
			var map = Dependency.Resolve<IMapper>();
			return map.Map<Logic.Contracts.DTO.Machine>(from);
		}

		public static Machine Convert(this Logic.Contracts.DTO.Machine from)
		{
			var map = Dependency.Resolve<IMapper>();
			return map.Map<Machine>(from);
		}
	}
}
