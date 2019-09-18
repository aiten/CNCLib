/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) Herbert Aitenbichler

  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
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
                throw new ArgumentException("Register name must not contain a blank");
            }

            _commandTypes.Add(name, shape);
        }

        public Command Create(string name)
        {
            if (name.Contains(" "))
            {
                throw new ArgumentException("Command name must not contain a blank");
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
            string commandName = char.ToUpper(name[0]) == 'M' ? "MXX" : "GXX";
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