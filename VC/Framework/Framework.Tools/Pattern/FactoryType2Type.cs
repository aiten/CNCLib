////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

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

namespace Framework.Tools.Pattern
{
	public class FactoryType2Type : IFactory
	{
		public TInterface Create<TInterface>() where TInterface : IDisposable
		{
			Type to;
			if (_typ2typeemapping.TryGetValue(typeof(TInterface),out to) == false)
			{
				throw new ArgumentException("Invalid InterfaceType, not mapped");
			}
			return (TInterface)Activator.CreateInstance(to);
		}

		private Dictionary<Type, Type> _typ2typeemapping = new Dictionary<Type, Type>();

		public void Register(Type from, Type to)
		{
			if (_typ2typeemapping.ContainsKey(from) == true)
			{
				throw new ArgumentException();
			}
			_typ2typeemapping[from] = to;
		}
	}
}
