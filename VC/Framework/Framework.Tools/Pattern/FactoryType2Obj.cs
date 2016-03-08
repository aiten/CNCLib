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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Tools.Pattern;

namespace Framework.Tools.Pattern
{
	public class FactoryType2Obj : IFactory
	{
		public TInterface Create<TInterface>() where TInterface : IDisposable
		{
			IDisposable to;
			if (_type2objmapping.TryGetValue(typeof(TInterface),out to) == false)
			{
				throw new ArgumentException("Invalid InterfaceType, not mapped");
			}
			return (TInterface) to;
		}

		private Dictionary<Type, IDisposable> _type2objmapping = new Dictionary<Type, IDisposable>();

		public void Register(Type from, IDisposable to)
		{
			_type2objmapping[from] = to;
		}
	}
}
