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


namespace CNCLib.GCode.Commands
{
	[IsGCommand("MXX")]
    class MxxCommand : Command
    {
        #region crt + factory

        public MxxCommand()
        {
            Code = "";
        }

        #endregion

        #region GCode
        public override void SetCode(string code ) { Code = code;  }

        #endregion

        #region Draw
        public override void Draw(IOutputCommand output, CommandState state, object param)
        {
            //base.Draw(output, state, param);

            switch (Code.ToUpper())
            {
                case "M106": state.LaserOn = true; state.UseLaser = true;  break;
                case "M107": state.LaserOn = false; break;
            }
        }

        #endregion
    }
}
