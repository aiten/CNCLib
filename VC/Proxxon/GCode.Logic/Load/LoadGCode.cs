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

using Framework.Tools;
using GCode.Logic.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCode.Logic.Load
{
    public class LoadGCode
    {
		CommandList _commands;
		CommandStream _stream = new CommandStream();
		Commands.CommandFactory _commandfactory = new Commands.CommandFactory();
		bool _wasg1;

		public LoadInfo LoadOptions { get; set; }

		public void Load(CommandList commands)
        {
			_commands = commands;
			commands.Clear();
			_wasg1 = true;

            using (StreamReader sr = new StreamReader(LoadOptions.FileName))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    _stream.Line = line;
                    if (!Command())
                    {
						_commands.Clear();
                        break;
                    }
                }
            }
			commands.UpdateCache();
        }

		private bool Command()
        {
			if (_stream.NextCharToUpper == 'N')
			{
				_stream.Next();
				while (char.IsDigit(_stream.NextChar))
				{
					_stream.Next();
				}
				_stream.SkipSpaces();
			}

			_stream.SkipSpaces();
			Command cmd;

			if (_stream.NextCharToUpper == 'G')
			{
				string cmdname = "G";
				_stream.Next();
				while (char.IsDigit(_stream.NextChar))
				{
					cmdname += _stream.NextChar;
					_stream.Next();
				}

				cmd = _commandfactory.Create(cmdname);

				if (cmd!=null)
				{
					if (cmd.GetType() == typeof(G01Command))
					{
						_wasg1 = true;
					}
					if (cmd.GetType() == typeof(G00Command))
					{
						_wasg1 = false;
					}

					_commands.AddCommand(cmd);
					if (!cmd.ReadFrom(_stream))
						return false;
				}
				else
				{
					cmd = _commandfactory.Create("GXX");
					((GxxCommand) cmd).Code = cmdname;

					_commands.AddCommand(cmd);
					if (!cmd.ReadFrom(_stream))
						return false;
				}
            }
			else if ("XYZABCF".IndexOf(_stream.NextCharToUpper) >= 0)
			{
				// g without prefix

				cmd = _commandfactory.Create(_wasg1 ? "G01" : "G00");

				if (cmd != null)
				{
					_commands.AddCommand(cmd);
					if (!cmd.ReadFrom(_stream))
						return false;
				}
			}
			else
			{
				cmd = _commandfactory.Create("GXX");

				_commands.AddCommand(cmd);
				if (!cmd.ReadFrom(_stream))
					return false;
			}

            return true;
        }
    }
}
