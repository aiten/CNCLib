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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CNCLib.GCode.Commands
{
    public class CommandFactory
    {
		private Dictionary<string, Type> _shapes = new Dictionary<string, Type>(); 
        
		public CommandFactory()
		{
			RegisterAll();
		}

		public void RegisterShape(string name, Type shape)
        {
            if (name.Contains(" "))
                throw new ArgumentException();
            _shapes.Add(name, shape);
        }

        public Command Create(string name)
        {
            if (name.Contains(" "))
            {
                throw new ArgumentException();
            }

            if (IsRegistered(name))
			{
				Type shape = _shapes[name.ToUpper()];
				return (Command)Activator.CreateInstance(shape);
			}
			return null;
        }
        public Command CreateOrDefault(string name)
        {
            string commandname = char.ToUpper(name[0]) == 'M' ? "MXX" : "GXX";
            int spaceidx = name.IndexOf(' ');
            if (spaceidx >= 0)
            {
                string tmpcommandname = name.Substring(0, spaceidx);
                if (IsRegistered(tmpcommandname))
                {
                    commandname = tmpcommandname;
                    name = name.Substring(spaceidx + 1);
                }
            }
            else if (IsRegistered(name))
            {
                commandname = name;
                name = "";
            }

            Command r = Create(commandname);
            if (name.Length > 0)
                r.GCodeAdd = name;

            return r;
        }

        public string[] GetKeys()
        {
            return _shapes.Keys.ToArray();
        }

		public bool IsRegistered(string name)
		{
			return _shapes.ContainsKey(name.ToUpper());
		} 

		private void RegisterAll()
		{
			Assembly ass = Assembly.GetExecutingAssembly();

			foreach (Type t in ass.GetTypes())
			{
				if (t.IsClass)
				{
					var isgcode = t.GetCustomAttribute<IsGCommandAttribute>();
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
