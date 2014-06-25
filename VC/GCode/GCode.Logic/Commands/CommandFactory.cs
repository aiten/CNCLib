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
using System.Diagnostics;
using System.Reflection;

namespace GCode.Logic.Commands
{
    public class CommandFactory
    {
		private Dictionary<String, Type> _shapes = new Dictionary<String, Type>(); 
        
		public CommandFactory()
		{
			RegisterAll();
		}

		public void RegisterShape(String name, Type shape)
        {
            _shapes.Add(name, shape);
        }

        public Command Create(string name)
        {
			if (IsRegistered(name))
			{
				Type shape = _shapes[name];
				return (Command)Activator.CreateInstance(shape); ;
			}
			return null;
        }

        public string[] GetKeys()
        {
            return _shapes.Keys.ToArray<String>();
        }

		public bool IsRegistered(string name) { return _shapes.ContainsKey(name); } 

		private void RegisterAll()
		{
			Assembly ass = Assembly.GetExecutingAssembly();

			foreach (Type t in ass.GetTypes())
			{
				if (t.IsClass)
				{
					IsGCommandAttribute isgcode = t.GetCustomAttribute<IsGCommandAttribute>();
					if (isgcode != null)
					{
						string ascodes = isgcode.RegisterAs;
						if (string.IsNullOrEmpty(ascodes))
							ascodes = t.Name.Substring(0,3);

						foreach (string ascode in ascodes.Split(','))
						{
							RegisterShape(ascode, t);
						}
					}
				}
			}
		}
    }
}
