////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2014 Herbert Aitenbichler

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proxxon.GCode.Commands
{
	[System.AttributeUsage(System.AttributeTargets.Class)]
	public class IsGCommandAttribute : Attribute
	{
		public bool IsGComamnd { get; set; }
		public String RegisterAs { get; set; }
		public IsGCommandAttribute()
		{
			IsGComamnd = true;
		}
		public IsGCommandAttribute(string registeras)
		{
			RegisterAs = registeras;
		}
	}
}
