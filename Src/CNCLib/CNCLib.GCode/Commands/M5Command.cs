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


namespace CNCLib.GCode.Commands
{
    [IsGCommand("M5")]
    public class M5Command : Command
    {
        #region crt + factory

        public M5Command()
        {
            Code = "M5";
        }

        #endregion

        #region GCode

        #endregion

        #region Itteration

        public override void SetCommandState(CommandState state)
        {
            base.SetCommandState(state);
            state.LaserOn = false;
        }

        #endregion
    }
}