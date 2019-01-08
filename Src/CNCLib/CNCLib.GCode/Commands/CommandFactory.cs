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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CNCLib.GCode.Commands
{
    public class CommandFactory
    {
        private readonly Dictionary<string, Type> _commandTypes = new Dictionary<string, Type>();

        public CommandFactory()
        {
            RegisterAll();
        }

        public void RegisterCommandType(string name, Type shape)
        {
            if (name.Contains(" "))
            {
                throw new ArgumentException();
            }

            _commandTypes.Add(name, shape);
        }

        public Command Create(string name)
        {
            if (name.Contains(" "))
            {
                throw new ArgumentException();
            }

            if (IsRegistered(name))
            {
                Type shape = _commandTypes[name.ToUpper()];
                return (Command)Activator.CreateInstance(shape);
            }

            return null;
        }

        public Command CreateOrDefault(string name)
        {
            string commandName = char.ToUpper(name[0]) == 'M'?"MXX":"GXX";
            int    spaceIdx    = name.IndexOf(' ');
            if (spaceIdx >= 0)
            {
                string tmpCommandName = name.Substring(0, spaceIdx);
                if (IsRegistered(tmpCommandName))
                {
                    commandName = tmpCommandName;
                    name        = name.Substring(spaceIdx + 1);
                }
            }
            else if (IsRegistered(name))
            {
                commandName = name;
                name        = "";
            }

            Command r = Create(commandName);
            if (name.Length > 0)
            {
                r.GCodeAdd = name;
            }

            return r;
        }

        public string[] GetKeys()
        {
            return _commandTypes.Keys.ToArray();
        }

        public bool IsRegistered(string name)
        {
            return _commandTypes.ContainsKey(name.ToUpper());
        }

        private void RegisterAll()
        {
            Assembly ass = Assembly.GetExecutingAssembly();

            foreach (Type t in ass.GetTypes())
            {
                if (t.IsClass)
                {
                    var isGcode = t.GetCustomAttribute<IsGCommandAttribute>();
                    if (isGcode != null)
                    {
                        string asCodes = isGcode.RegisterAs;
                        if (string.IsNullOrEmpty(asCodes))
                        {
                            asCodes = t.Name.Substring(0, 3);
                        }

                        foreach (string asCode in asCodes.Split(','))
                        {
                            RegisterCommandType(asCode, t);
                        }
                    }
                }
            }
        }
    }
}