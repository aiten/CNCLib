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

using Framework.Tools.Helpers;

///////////////////////////////////////////////////////

namespace CNCLib.GCode.Parser
{
    public class GCodeParser : Framework.Tools.Parser.Parser
    {
        public GCodeParser(CommandStream reader) : base(reader) { }

        public override void Parse()
        {

        }
    }
}