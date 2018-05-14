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
*/
////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////
//
//#define EXPRPARSER_MAXTOKENLENGTH 16

using System;
using System.IO;
using Framework.Tools.Helpers;

////////////////////////////////////////////////////////

namespace Framework.Tools.Parser
{
    public abstract class Parser
    {
        private string _error;

        public bool IsError()
        {
            return !string.IsNullOrEmpty(_error);
        }

        protected void ErrorAdd(string error)
        {
            if (string.IsNullOrEmpty(_error))
            {
                Error(error);
            }
            else
            {
                Error(_error + "\n" + error);
            }
        }

        protected void Error(string error)
        {
            _error = error;
        }

        public Parser(CommandStream reader)
        {
            _reader = reader;
        }

        public abstract void Parse();

        protected CommandStream _reader;

        protected bool IsToken(string b, bool expectdel, bool ignorecase)
        {
            return _reader.IsCommand(b);
        }
    }
}