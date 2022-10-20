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

namespace CNCLib.GCode.Machine;

public class EepromV1Plotter : EepromV1
{
    #region Properties

    public const uint SIGNATUREPLOTTER_V1 = 0x21438702;

    public override uint VersionSignature => SIGNATUREPLOTTER_V1;

    #endregion

    #region Read/Write

    public override void ReadFrom(Eeprom eeprom)
    {
        base.ReadFrom(eeprom);

        EepromVxPlotterExtensions.ReadPlotter(this, eeprom);
    }

    public override void WriteTo(Eeprom eeprom)
    {
        base.WriteTo(eeprom);

        EepromVxPlotterExtensions.WritePlotter(this, eeprom);
    }

    #endregion
}