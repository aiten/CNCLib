﻿/*
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

namespace CNCLib.WpfClient.Helpers;

using System;

using CNCLib.GCode.Tools;

using MachineDto = CNCLib.Logic.Abstraction.DTO.Machine;

public static class Extensions
{
    public static string GetAxisName(this MachineDto m, int axis)
    {
        return GCodeHelper.IndexToAxisName(axis);
    }

    public static decimal GetSize(this MachineDto m, int axis)
    {
        switch (axis)
        {
            case 0: return m.SizeX;
            case 1: return m.SizeY;
            case 2: return m.SizeZ;
            case 3: return m.SizeA;
            case 4: return m.SizeB;
            case 5: return m.SizeC;
        }

        throw new NotImplementedException();
    }

    public static decimal GetProbeSize(this MachineDto m, int axis)
    {
        switch (axis)
        {
            case 0: return m.ProbeSizeX;
            case 1: return m.ProbeSizeY;
            case 2: return m.ProbeSizeZ;
        }

        return 0m;
    }
}