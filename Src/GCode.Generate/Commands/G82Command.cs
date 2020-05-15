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

using System;
using System.Collections.Generic;

namespace CNCLib.GCode.Generate.Commands
{
    [IsGCommand]
    public class G82Command : DrillCommand
    {
        #region crt + factory

        public G82Command()
        {
            Code = "G82";
        }

        #endregion

        #region Convert

        public override Command[] ConvertCommand(CommandState state, ConvertOptions options)
        {
            if (!options.SubstG82)
            {
                return base.ConvertCommand(state, options);
            }

            // from
            // G82 X-8.8900 Y3.8100  Z-0.2794 F400 R0.5000  P0.000000
            // next command with R and P G82 X-11.4300 Y3.8100
            // to
            // G00 X-8.8900 Y3.8100
            // G01 Z-0.2794 F400
            // (G04 P0)
            // G00 Z0.5000

            Variable r = GetVariable('R');
            if (r == null)
            {
                r = state.G82R;
            }
            else
            {
                state.G82R = r.ShallowCopy();
            }

            Variable p = GetVariable('P');
            if (p == null)
            {
                p = state.G82P;
            }
            else
            {
                state.G82P = p.ShallowCopy();
            }

            Variable z = GetVariable('Z');
            if (z == null)
            {
                z = state.G82Z;
            }
            else
            {
                state.G82Z = z.ShallowCopy();
            }

            var list = new List<Command>();

            var move1 = new G00Command();
            CopyVariable('X', move1);
            CopyVariable('Y', move1);
            list.Add(move1);

            var move2 = new G01Command();
            move2.AddVariable('Z', z);
            CopyVariable('F', move2);
            list.Add(move2);

            if (p != null && Math.Abs(p.Value ?? 0.0) > double.Epsilon)
            {
                var move3 = new G04Command();
                move3.AddVariable('P', p);
                list.Add(move3);
            }

            var move4 = new G00Command();
            move4.AddVariable('Z', r);
            list.Add(move4);

            return list.ToArray();
        }

        #endregion

        #region Draw

        #endregion
    }
}