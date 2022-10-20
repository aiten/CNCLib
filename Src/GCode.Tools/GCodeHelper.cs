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

namespace CNCLib.GCode.Tools;

using System;

public class GCodeHelper
{
    public static int AxisNameToIndex(string axisName)
    {
        if (axisName.Length == 1)
        {
            return AxisNameToIndex(axisName[0]);
        }

        return -1;
    }

    public static int AxisNameToIndex(char axisName)
    {
        switch (char.ToUpper(axisName))
        {
            case 'X': return 0;
            case 'Y': return 1;
            case 'Z': return 2;
            case 'A': return 3;
            case 'B': return 4;
            case 'C': return 5;
        }

        return -1;
    }

    public static string IndexToAxisName(int axis)
    {
        switch (axis)
        {
            case 0: return "X";
            case 1: return "Y";
            case 2: return "Z";
            case 3: return "A";
            case 4: return "B";
            case 5: return "C";
        }

        throw new ArgumentOutOfRangeException(nameof(axis), axis, @"axis index must be < 6");
    }
}