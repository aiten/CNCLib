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

using Framework.Tools;
using CNCLib.GCode.Commands;
using System.IO;

namespace CNCLib.GCode.Load
{
    public class LoadGCode : LoadBase
    {
        CommandStream _stream = new CommandStream();
        Command _lastnoPrefixCommand;

        public override void Load()
        {
			PreLoad();

			_lastnoPrefixCommand = null;

            using (StreamReader sr = new StreamReader(LoadOptions.FileName))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    _stream.Line = line;
                    if (!Command())
                    {
                        Commands.Clear();
                        break;
                    }
                }
            }

			PostLoad();
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

            if (_stream.NextCharToUpper == 'G')
            {
                if (!ReadGCommand())
                    return false;
            }
            else if ("XYZABCF".IndexOf(_stream.NextCharToUpper) >= 0)
            {
                if (_lastnoPrefixCommand == null || !ReadGNoPrefixCommand())
                    return false;
            }
            else if (_stream.NextCharToUpper == 'M')
            {
                if (!ReadMCommand())
                    return false;
            }
            else
            {
                if (!ReadOtherCommand())
                    return false;
            }

            return true;
        }

        private bool AddGxxMxxCommand(Command cmd, string cmdname)
        {
            cmd.SetCode(cmdname);

            Commands.AddCommand(cmd);
            if (!cmd.ReadFrom(_stream))
                return false;

            return true;
        }

        private bool ReadGCommand()
        {
            _stream.Next();

            string cmdname = "G" + _stream.ReadDigits();
            _stream.SkipSpaces();

            Command cmd = CommandFactory.Create(cmdname);

            if (cmd != null)
            {
                if (cmd.UseWithoutPrefix)
                    _lastnoPrefixCommand = cmd;

                Commands.AddCommand(cmd);
                if (!cmd.ReadFrom(_stream))
                    return false;
            }
            else
            {
                if (!AddGxxMxxCommand(CommandFactory.Create("GXX"), cmdname))
                    return false;
            }

            return true;
        }

        private bool ReadGNoPrefixCommand()
        {
            // g without prefix

            Command cmd = CommandFactory.Create(_lastnoPrefixCommand.Code);

            if (cmd != null)
            {
                Commands.AddCommand(cmd);
                if (!cmd.ReadFrom(_stream))
                    return false;
            }
            return true;
        }

        private bool ReadMCommand()
        {
            _stream.Next();
            string cmdname = "M" + _stream.ReadDigits();
            _stream.SkipSpaces();

            Command cmd = CommandFactory.Create(cmdname);

            if (cmd != null)
            {
                Commands.AddCommand(cmd);
                if (!cmd.ReadFrom(_stream))
                    return false;
            }
            else
            {
                if (!AddGxxMxxCommand(CommandFactory.Create("MXX"), cmdname))
                    return false;
            }
            return true;
        }

        private bool ReadOtherCommand()
        {
            Command cmd = CommandFactory.Create("GXX");

            Commands.AddCommand(cmd);
            if (!cmd.ReadFrom(_stream))
                return false;

            return true;
        }
    }
}

