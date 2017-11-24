////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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
using System.IO;
using CNCLib.GCode.Commands;
using Framework.Tools.Helpers;

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

            using (StreamReader sr = GetStreamReader())
            {
				try
				{
					string line;
					while ((line = sr.ReadLine()) != null)
					{
						_stream.Line = line;
						Command();
					}
				}
				catch (FormatException)
				{
					Commands.Clear();
				}
			}

			PostLoad();
		}

		public void Load(string[] lines)
		{
			_lastnoPrefixCommand = null;

			try
			{
				foreach (var line in lines)
				{
					_stream.Line = line;
					Command();
				}
			}
			catch (FormatException)
			{
				Commands.Clear();
			}

			PostLoad();
		}

		private void Command()
        {
			int? linenumber=null;

            if (_stream.NextCharToUpper == 'N')
            {
				_stream.Next();

				if (!_stream.IsNumber())
					throw new FormatException();

				linenumber = _stream.GetInt();
				_stream.SkipSpaces();
            }

            _stream.SkipSpaces();
			Command cmd;

            if (_stream.NextCharToUpper == 'G')
            {
				cmd = ReadGCommand();
            }
            else if ("XYZABCF".IndexOf(_stream.NextCharToUpper) >= 0)
            {
				if (_lastnoPrefixCommand == null)
					throw new FormatException();

				cmd = ReadGNoPrefixCommand();
            }
            else if (_stream.NextCharToUpper == 'M')
            {
				cmd = ReadMCommand();
            }
            else
            {
				cmd = ReadOtherCommand();
            }

			if (cmd != null)
			{
				if (linenumber.HasValue)
				{
					cmd.LineNumber = linenumber;
				}

				Commands.AddCommand(cmd);
			}
        }

        private Command AddGxxMxxCommand(Command cmd, string cmdname)
        {
            cmd.SetCode(cmdname);
			cmd.ReadFrom(_stream);
            return cmd;
        }

        private Command ReadGCommand()
        {
            _stream.Next();

            string cmdname = "G" + _stream.ReadDigits();
            _stream.SkipSpaces();

            Command cmd = CommandFactory.Create(cmdname);

            if (cmd != null)
            {
                if (cmd.UseWithoutPrefix)
                    _lastnoPrefixCommand = cmd;

				cmd.ReadFrom(_stream);
            }
            else
            {
				cmd = AddGxxMxxCommand(CommandFactory.Create("GXX"), cmdname);
            }

            return cmd;
        }

        private Command ReadGNoPrefixCommand()
        {
            // g without prefix

            Command cmd = CommandFactory.Create(_lastnoPrefixCommand.Code);
            if (cmd != null)
            {
				cmd.ReadFrom(_stream);
            }
            return cmd;
        }

        private Command ReadMCommand()
        {
            _stream.Next();
            string cmdname = "M" + _stream.ReadDigits();
            _stream.SkipSpaces();

            Command cmd = CommandFactory.Create(cmdname);

            if (cmd != null)
            {
				cmd.ReadFrom(_stream);
            }
            else
            {
				cmd = AddGxxMxxCommand(CommandFactory.Create("MXX"), cmdname);
            }
            return cmd;
        }

        private Command ReadOtherCommand()
        {
            Command cmd = CommandFactory.Create("GXX");
			cmd.ReadFrom(_stream);
            return cmd;
        }
    }
}

